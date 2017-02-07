using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public partial class ServerOnClient
    {
        private readonly Queue<SendableMessage>[] _udpQueues = {
            new Queue<SendableMessage>(16),
            new Queue<SendableMessage>(16)
        };

        private int _batchingUdpQueueIndex;
        private Thread _udpWorker;

        private readonly AutoResetEvent _udpWorkerResetEvent = new AutoResetEvent(false);

        private readonly object _batchedUdpLock = new object();

        private int _udpSendIndex;
        private byte[] _udpSendBuffer;

        readonly byte[] _udpReadBuffer = new byte[NetworkHelper.InitialUdpBufferSize];

        private void UdpWorkerLoop()
        {
            while (_keepRunning)
            {
                _udpWorkerResetEvent.WaitOne(250);

                if(!_flushing)
                    continue;


                if (_udpQueues[_batchingUdpQueueIndex].Count > 0)
                {
                    Queue<SendableMessage> queue = null;

                    lock (_batchedUdpLock)
                    {
                        queue = _udpQueues[_batchingUdpQueueIndex];

                        if (++_batchingUdpQueueIndex >= _udpQueues.Length)
                            _batchingUdpQueueIndex = 0;
                    }

                    var lastPosition = -1;

                    while (queue.Count > 0)
                    {
                        var msg = queue.Dequeue();

                        NetworkHelper.Serialize(msg, ref _udpSendBuffer, ref _udpSendIndex);
                        NetworkHelper.MaybeSendUdp(_udpClient.Client, ref _udpSendBuffer, ref _udpSendIndex, ref lastPosition, RemoteAddress2);
                    }

                    if (lastPosition > -1)
                    {
                        _udpClient.Client.SendTo(_udpSendBuffer, 0, lastPosition + 1, SocketFlags.None, RemoteAddress2);
                        _udpSendIndex = 0;
                    }
                }
            }
        }

        private void OnUdpRead(IAsyncResult ar)
        {
            var read = _udpClient.Client.EndReceiveFrom(ar, ref RemoteAddress2);
            NetworkHelper.ProcessData(HandleData, _udpReadBuffer, read);
            _udpClient.Client.BeginReceiveFrom(_udpReadBuffer, 0, _udpReadBuffer.Length, SocketFlags.None, ref RemoteAddress2, OnUdpRead, null);
        }
    }
}