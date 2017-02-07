using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Misana.Core.Client;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public partial class ServerOnClient : IServerOnClient
    {
        public int NetworkId { get; } = NetworkManager.GetNextId();

        private readonly UdpClient _udpClient;

        public bool CanConnect { get; private set; }
        public bool IsConnected { get; private set; }
        public IClientRpcMessageHandler ClientRpcHandler { get; set; }
        public bool TryDequeue(out IGameMessage msg)
        {
            if (_gameMessageQueue.Count == 0)
            {
                msg = null;
                return false;
            }

            lock (_gameMessageLock)
            {
                msg = _gameMessageQueue.Dequeue();
                return true;
            }
        }

        public bool CanSend { get; } = true;

        public IPAddress RemoteAddress { get; private set; }
        public EndPoint RemoteAddress2;

        private readonly int sendPort;

        public ServerOnClient()
        {
            sendPort = NetworkManager.ServerUdpPort;
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any,NetworkManager.LocalUdpPort));
            _udpClient.DontFragment = true;
            _udpSendBuffer = new byte[NetworkHelper.InitialUdpBufferSize];
            CanConnect = true;
        }


        private bool _keepRunning;

        private bool _flushing;


        private Queue<IGameMessage> _gameMessageQueue = new Queue<IGameMessage>();
        private readonly object _gameMessageLock = new object();


        public void Start()
        {
            _keepRunning = true;
            _tcpHandler.Start();

            _udpClient.Client.BeginReceiveFrom(_udpReadBuffer, 0, _udpReadBuffer.Length, SocketFlags.None, ref RemoteAddress2, OnUdpRead, null);
            _udpWorker = new Thread(UdpWorkerLoop){ IsBackground = true };
            _udpWorker.Start();
        }

        public void HandleData(byte[] data, ref int processed)
        {
            var msgId = Deserializer.ReadByte(data, ref processed);
            var msgType = Deserializer.ReadInt32(data, ref processed);

            var msg = MessageInfo.Deserializers[msgType](data, ref processed);

            if(MessageInfo.IsRpcMessage[msgType])
                ((IRpcMessage)msg).HandleOnClient(ClientRpcHandler);
            else
            {
                lock (_gameMessageLock)
                {
                    _gameMessageQueue.Enqueue((IGameMessage) msg);
                }
            }
        }

        private TcpHandler _tcpHandler;

        public void FlushQueue()
        {
            _flushing = true;
            _tcpHandler?.Flush();
            _udpWorkerResetEvent.Set();
        }

        public void Enqueue<T>(T message) where T : NetworkMessage
        {
            if (MessageInfo<T>.IsUdp)
            {

                lock (_batchedUdpLock)
                {
                    _udpQueues[_batchingUdpQueueIndex]
                        .Enqueue(
                            new SendableMessage {
                                Message = message,
                                MessageId = MessageInfo<T>.NextMessageId(),
                                MessageType = MessageInfo<T>.Index,
                                Serialize = MessageInfo<T>.SerializeBase
                            });
                }
            }
            else
            {
                _tcpHandler.Enqueue(new SendableMessage {
                    Message = message,
                    MessageId = MessageInfo<T>.NextMessageId(),
                    MessageType = MessageInfo<T>.Index,
                    Serialize = MessageInfo<T>.SerializeBase
                });
            }
        }


        public void Request<TRequest>(TRequest request) where TRequest : NetworkRequest
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");


            if(MessageInfo<TRequest>.IsUdp)
                throw new InvalidOperationException();

            _tcpHandler.Enqueue(new SendableMessage {
                Message = request,
                MessageId = MessageInfo<TRequest>.NextMessageId(),
                MessageType = MessageInfo<TRequest>.Index,
                Serialize = MessageInfo<TRequest>.SerializeBase
            });
        }

        private async Task Connect(IPEndPoint endPoint)
        {
            if (IsConnected || !CanConnect)
                throw new InvalidOperationException("Client is connected");

            RemoteAddress = endPoint.Address;
            RemoteAddress2 = new IPEndPoint(endPoint.Address, sendPort);

            var client = new TcpClient();
            await client.ConnectAsync(endPoint.Address, endPoint.Port);
            _tcpHandler = new TcpHandler(client, HandleData, () => {});

            Start();

            IsConnected = true;
        }

        public Task Connect(IPAddress addr)
        {
            return Connect(new IPEndPoint(addr, NetworkManager.TcpPort));
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;

            //_tcpClient.Close();
        }

        void IOutgoingMessageQueue.Enqueue<T>(T msg)
        {
            Enqueue(msg);
        }
    }
}