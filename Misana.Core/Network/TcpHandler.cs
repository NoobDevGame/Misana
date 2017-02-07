using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Misana.Core.Network
{
    public class TcpHandler
    {
        private readonly DataHandler _handler;
        private readonly Action _onDisconnect;

        public TcpHandler(DataHandler handler, Action onDisconnect)
            : this(new TcpClient(), handler, onDisconnect)
        {
        }

        public TcpHandler(TcpClient client, DataHandler handler, Action onDisconnect)
        {
            _tcpClient = client;
            _handler = handler;
            _onDisconnect = onDisconnect;
            _tcpSendBuffer = new byte[NetworkHelper.InitialTcpBufferSize];
        }

        private readonly object _immediateLock = new object();
        private readonly TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly byte[] _tcpReadBuffer = new byte[NetworkHelper.InitialTcpBufferSize];
        private readonly byte[] _tcpOverflowBuffer = new byte[NetworkHelper.InitialTcpBufferSize];
        private readonly Queue<SendableMessage> _immediateTcpQueue = new Queue<SendableMessage>();
        private readonly Queue<SendableMessage> _batchingTcpQueue = new Queue<SendableMessage>();

        private Thread _tcpWorker;
        private readonly AutoResetEvent _tcpWorkerResetEvent = new AutoResetEvent(false);

        private int _tcpSendIndex;
        private int _tcpOverflowIndex;
        private int _overflowExpectedLength;
        private bool _tcpOverflow;
        private byte[] _tcpSendBuffer;

        private readonly object _batchedTcpLock = new object();


        private bool KeepRunning;

        public void Enqueue(SendableMessage msg)
        {
            lock (_batchedTcpLock)
            {
                _batchingTcpQueue.Enqueue(msg);
            }
        }

        public void Send(SendableMessage msg)
        {
            lock (_immediateLock)
            {
                _immediateTcpQueue.Enqueue(msg);
            }
            _tcpWorkerResetEvent.Set();
        }

        public void Start()
        {
            _stream = _tcpClient.GetStream();
            _stream.BeginRead(_tcpReadBuffer, 0, _tcpReadBuffer.Length, OnTcpRead, null);
            KeepRunning = true;
            _tcpWorker = new Thread(TcpWorkerLoop) {
                IsBackground = true
            };
            _tcpWorker.Start();
        }

        private void OnTcpRead(IAsyncResult ar)
        {
            try
            {
                var read = _stream.EndRead(ar);
                NetworkHelper.ProcessData(_handler, _tcpReadBuffer, read, ref _tcpOverflow, ref _overflowExpectedLength, _tcpOverflowBuffer, ref _tcpOverflowIndex);
                _stream.BeginRead(_tcpReadBuffer, 0, _tcpReadBuffer.Length, OnTcpRead, null);

            }
            catch (IOException e)
            {
                if (e.InnerException != null && e.InnerException is SocketException)
                {
                    var sockE = (SocketException) e.InnerException;
                    if (sockE.ErrorCode == (int) SocketError.ConnectionReset)
                    {
                        _onDisconnect?.Invoke();
                        KeepRunning = false;
                        return;
                    }
                }

                throw;
            }
        }

        private void TcpWorkerLoop()
        {
            while (KeepRunning)
            {
                _tcpWorkerResetEvent.WaitOne(250);
                var found = false;

                if (_immediateTcpQueue.Count > 0)
                {
                    lock (_immediateLock)
                    {
                        if (_immediateTcpQueue.Count > 0)
                        {
                            var msg = _immediateTcpQueue.Dequeue();
                            NetworkHelper.Serialize(msg, ref _tcpSendBuffer, ref _tcpSendIndex);
                            found = true;
                        }
                    }
                }

                if (found)
                {
                    _tcpClient.Client.Send(_tcpSendBuffer, 0, _tcpSendIndex + 1, SocketFlags.None);
                    _tcpSendIndex = 0;
                }
            }
        }

        public void Flush()
        {
            lock (_immediateTcpQueue)
            {
                lock (_batchedTcpLock)
                {
                    while (_batchingTcpQueue.Count > 0)
                    {
                        _immediateTcpQueue.Enqueue(_batchingTcpQueue.Dequeue());
                    }
                }
            }
            _tcpWorkerResetEvent.Set();
        }
    }
}