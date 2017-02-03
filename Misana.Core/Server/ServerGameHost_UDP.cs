using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Misana.Core.Network;
using Misana.Serialization;

namespace Misana.Core.Server
{
    public partial class ServerGameHost
    {
        private Queue<SendableServerMessage>[] _udpQueues = {
            new Queue<SendableServerMessage>(16),
            new Queue<SendableServerMessage>(16)
        };
        readonly byte[] udpBuffer = new byte[1500];
        private int _batchingUdpQueueIndex;
        private Thread _udpWorker;
        private EndPoint _udpEndPoint;
        private AutoResetEvent _udpWorkerResetEvent = new AutoResetEvent(false);

        private readonly object _batchedUdpLock = new object();

        private int _udpSendIndex;
        private byte[] _udpSendBuffer;
        private UdpClient _udpClient;


        public void EnqueueUdp<T>(T message, IClientOnServer clientOnServer) where T : NetworkMessage, IGameMessage
        {
            lock (_batchedUdpLock)
            {
                var queue = _udpQueues[_batchingUdpQueueIndex];
                queue.Enqueue(new SendableServerMessage {
                    Client = clientOnServer,
                    Message = message,
                    MessageId = MessageInfo<T>.NextMessageId(),
                    MessageType = MessageInfo<T>.Index,
                    Serialize = MessageInfo<T>.SerializeBase
                });
            }
        }

        private void UdpWorkerLoop()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                _udpWorkerResetEvent.WaitOne(250);

                if(!_flushing)
                    continue;


                if (_udpQueues[_batchingUdpQueueIndex].Count > 0)
                {
                    Queue<SendableServerMessage> queue = null;

                    lock (_batchedUdpLock)
                    {
                        queue = _udpQueues[_batchingUdpQueueIndex];

                        if (++_batchingUdpQueueIndex >= _udpQueues.Length)
                            _batchingUdpQueueIndex = 0;
                    }

                    const int cutOff = 500;

                    var lastPosition = -1;

                    EndPoint endPoint = null;
                    while (queue.Count > 0)
                    {
                        var msg = queue.Dequeue();

                        if (endPoint != msg.Client.UdpEndpoint)
                        {
                            if (lastPosition > -1)
                            {
                                _udpClient.Client.SendTo(_udpSendBuffer, 0, lastPosition + 1, SocketFlags.None, endPoint);
                                _udpSendIndex = 0;
                            }

                            endPoint = msg.Client.UdpEndpoint;
                        }

                        Serializer.EnsureSize(ref _udpSendBuffer, _udpSendIndex + 128 + 1);
                        Serializer.WriteByte(msg.MessageId, ref _udpSendBuffer, ref _udpSendIndex);
                        Serializer.WriteInt32(msg.MessageType, ref _udpSendBuffer, ref _udpSendIndex);

                        msg.Serialize(msg.Message, ref _udpSendBuffer, ref _udpSendIndex);

                        if (_udpSendIndex >= cutOff)
                        {
                            if (lastPosition == -1)
                            {
                                _udpClient.Client.SendTo(_udpSendBuffer, 0, _udpSendIndex + 1, SocketFlags.None, endPoint);
                                _udpSendIndex = 0;
                            }
                            else
                            {
                                _udpClient.Client.SendTo(_udpSendBuffer, 0, lastPosition + 1, SocketFlags.None, endPoint);
                                _udpClient.Client.SendTo(_udpSendBuffer, lastPosition + 1, _udpSendIndex + 1, SocketFlags.None, endPoint);

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
                        _udpClient.Client.SendTo(_udpSendBuffer, 0, lastPosition + 1, SocketFlags.None, endPoint);
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

                IClientOnServer client;
                if (!_ipClients.TryGetValue(((IPEndPoint) e).Address, out client))
                {
                    _udpClient.Client.BeginReceiveFrom(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, ref _udpEndPoint, OnUdpRead, null);
                    return;
                }

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

                        client.HandleData(udpBuffer, ref processed);


                    }

                    if (processed >= len)
                    {
                        break;
                    }
                }

                _udpClient.Client.BeginReceiveFrom(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, ref _udpEndPoint, OnUdpRead, null);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

}