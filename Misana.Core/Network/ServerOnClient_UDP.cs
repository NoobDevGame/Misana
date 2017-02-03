using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public partial class ServerOnClient
    {
        private Queue<SendableMessage>[] _udpQueues = {
            new Queue<SendableMessage>(16),
            new Queue<SendableMessage>(16)
        };

        private int _batchingUdpQueueIndex;
        private Thread _udpWorker;

        private AutoResetEvent _udpWorkerResetEvent = new AutoResetEvent(false);

        private readonly object _batchedUdpLock = new object();

        private int _udpSendIndex;
        private byte[] _udpSendBuffer;

        readonly byte[] udpBuffer = new byte[1500];

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

                    const int cutOff = 500;

                    var lastPosition = -1;

                    while (queue.Count > 0)
                    {
                        var msg = queue.Dequeue();
                        Serializer.EnsureSize(ref _udpSendBuffer, _udpSendIndex + 128 + 1);
                        Serializer.WriteByte(msg.MessageId, ref _udpSendBuffer, ref _udpSendIndex);
                        Serializer.WriteInt32(msg.MessageType, ref _udpSendBuffer, ref _udpSendIndex);
                        msg.Serialize(msg.Message, ref _udpSendBuffer, ref _udpSendIndex);

                        if (_udpSendIndex >= cutOff)
                        {
                            if (lastPosition == -1)
                            {
                                _udpClient.Client.SendTo(_udpSendBuffer, 0, _udpSendIndex + 1, SocketFlags.None, RemoteAddress2);
                                _udpSendIndex = 0;
                            }
                            else
                            {
                                _udpClient.Client.SendTo(_udpSendBuffer, 0, lastPosition + 1, SocketFlags.None, RemoteAddress2);
                                _udpClient.Client.SendTo(_udpSendBuffer, lastPosition + 1, _udpSendIndex + 1, SocketFlags.None, RemoteAddress2);

                                _udpSendIndex = 0;
                            }
                            lastPosition = -1;
                        }
                        else
                        {
                            lastPosition = _udpSendIndex;
                        }
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
            try
            {
                EndPoint e = null;
                var read = _udpClient.Client.EndReceiveFrom(ar, ref e);

                var processed = 0;
                while (true)
                {

                    var len = Deserializer.ReadInt32(udpBuffer, ref processed);

                    if (read - processed < len)
                    {
                        throw new NotImplementedException("Nope");
                    }
                    else
                    {
                        HandleData(udpBuffer, ref processed);
                    }

                    if (processed >= len)
                    {
                        break;
                    }
                }

                _udpClient.Client.BeginReceiveFrom(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, ref RemoteAddress2, OnUdpRead, null);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}