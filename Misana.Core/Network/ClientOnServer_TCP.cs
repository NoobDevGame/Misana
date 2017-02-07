using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public partial class ClientOnServer
    {
        private readonly object _immediateLock = new object();
        private readonly TcpClient _tcpClient;
        private NetworkStream stream;
        readonly byte[] tcpBuffer = new byte[8192];
        private Queue<SendableMessage> _immediateTcpQueue = new Queue<SendableMessage>();
        private Queue<SendableMessage> _batchingTcpQueue = new Queue<SendableMessage>();

        private Thread _tcpWorker;
        private AutoResetEvent _tcpWorkerResetEvent = new AutoResetEvent(false);

        private int _tcpSendIndex;
        private byte[] _tcpSendBuffer;

        private readonly object _batchedTcpLock = new object();


        private void OnTcpRead(IAsyncResult ar)
        {
            try
            {
                var read = stream.EndRead(ar);

                var processed = 0;
                while (true)
                {
                    var len = Deserializer.ReadInt32(tcpBuffer, ref processed);

                    if (read - processed < len)
                    {
                        throw new NotImplementedException("Nope");
                    }
                    else
                    {
                        HandleData(tcpBuffer, ref processed);
                    }

                    if (processed >= len)
                    {
                        break;
                    }
                }

                stream.BeginRead(tcpBuffer, 0, tcpBuffer.Length, OnTcpRead, null);

            }
            catch (IOException e)
            {
                if (e.InnerException != null && e.InnerException is SocketException)
                {
                    var sockE = (SocketException) e.InnerException;
                    if (sockE.ErrorCode == (int) SocketError.ConnectionReset)
                    {
                        _server.OnDisconnectClient(this);
                        return;
                    }
                }

                throw;
            }
        }

        private void TcpWorkerLoop()
        {
            while (_keepRunning)
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
                            Serializer.EnsureSize(ref _tcpSendBuffer, _tcpSendIndex + 128);
                            var writeLengthTo = _tcpSendIndex;
                            _tcpSendIndex += 4;
                            Serializer.WriteByte(msg.MessageId, ref _tcpSendBuffer, ref _tcpSendIndex);
                            Serializer.WriteInt32(msg.MessageType, ref _tcpSendBuffer, ref _tcpSendIndex);
                            msg.Serialize(msg.Message, ref _tcpSendBuffer, ref _tcpSendIndex);
                            Serializer.WriteInt32(_tcpSendIndex - writeLengthTo + 1 - 4, ref _tcpSendBuffer, ref writeLengthTo);
                            found = true;
                        }
                    }
                }

                if (_flushing)
                {
                    if (_batchingTcpQueue.Count > 0)
                    {
                        lock (_batchedTcpLock)
                        {
                            while (_batchingTcpQueue.Count > 0)
                            {
                                var msg = _batchingTcpQueue.Dequeue();
                                Serializer.EnsureSize(ref _tcpSendBuffer, _tcpSendIndex + 128);
                                var writeLengthTo = _tcpSendIndex;
                                _tcpSendIndex += 4;
                                Serializer.WriteByte(msg.MessageId, ref _tcpSendBuffer, ref _tcpSendIndex);
                                Serializer.WriteInt32(msg.MessageType, ref _tcpSendBuffer, ref _tcpSendIndex);
                                msg.Serialize(msg.Message, ref _tcpSendBuffer, ref _tcpSendIndex);
                                Serializer.WriteInt32(_tcpSendIndex - writeLengthTo + 1 - 4, ref _tcpSendBuffer, ref writeLengthTo);
                                found = true;
                            }
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
    }
}