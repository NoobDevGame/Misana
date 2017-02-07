using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Misana.Core.Network;
using Misana.Serialization;

namespace Misana.Core.Server
{
    public partial class ServerGameHost
    {
        private readonly Queue<SendableServerMessage>[] _udpQueues = {
            new Queue<SendableServerMessage>(16),
            new Queue<SendableServerMessage>(16)
        };
        readonly byte[] _udpReadBuffer = new byte[NetworkHelper.InitialUdpBufferSize];
        private int _batchingUdpQueueIndex;
        private Thread _udpWorker;
        private EndPoint _udpEndPoint;
        private readonly AutoResetEvent _udpWorkerResetEvent = new AutoResetEvent(false);

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

                    const int cutOff = 1024;

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

                        NetworkHelper.Serialize(msg, ref _udpSendBuffer, ref _udpSendIndex);
                        NetworkHelper.MaybeSendUdp(_udpClient.Client, ref _udpSendBuffer, ref _udpSendIndex, ref lastPosition, endPoint);


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
            EndPoint e = new IPEndPoint(IPAddress.Any, NetworkManager.ServerUdpPort);
            try
            {

                var read = _udpClient.Client.EndReceiveFrom(ar, ref e);

                IClientOnServer client;
                if (!_ipClients.TryGetValue(((IPEndPoint) e).Address, out client))
                {
                    _udpClient.Client.BeginReceiveFrom(_udpReadBuffer, 0, _udpReadBuffer.Length, SocketFlags.None, ref _udpEndPoint, OnUdpRead, null);
                    return;
                }

                NetworkHelper.ProcessData(client.HandleData, _udpReadBuffer, read);

                _udpClient.Client.BeginReceiveFrom(_udpReadBuffer, 0, _udpReadBuffer.Length, SocketFlags.None, ref _udpEndPoint, OnUdpRead, null);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == (int) SocketError.ConnectionReset)
                {
                    //_udpClient.Client.
                    _udpClient.Client.BeginReceiveFrom(_udpReadBuffer, 0, _udpReadBuffer.Length, SocketFlags.None, ref _udpEndPoint, OnUdpRead, null);
                    var ip = (IPEndPoint) e;
                    var cli = GetClientByIp(ip.Address);
                    OnDisconnectClient(cli);
                }
                else
                {
                    throw;
                }
            }
        }
    }

}