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
        public bool IsServer { get; private set; }

        private readonly int sendPort;

        public ServerOnClient()
        {
            sendPort = NetworkManager.ServerUdpPort;
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any,NetworkManager.LocalUdpPort));
            _tcpClient = new TcpClient();
            _udpClient.DontFragment = true;
            _tcpSendBuffer = new byte[4096];
            _udpSendBuffer = new byte[1536];

            CanConnect = true;
        }

        internal ServerOnClient(TcpClient tcpClient,UdpClient udpClient,IPAddress address)
        {
            RemoteAddress = address;

            sendPort = NetworkManager.LocalUdpPort;
            _tcpClient = tcpClient;

            stream = tcpClient.GetStream();

            _tcpSendBuffer = new byte[4096];
            _udpSendBuffer = new byte[1536];
            _udpClient = udpClient;

            CanConnect = false;
            IsConnected = true;
            IsServer = true;
        }


        private bool _keepRunning;

        private bool _flushing;


        private Queue<IGameMessage> _gameMessageQueue = new Queue<IGameMessage>();
        private readonly object _gameMessageLock = new object();


        public void Start()
        {
            _keepRunning = true;
            stream.BeginRead(tcpBuffer, 0, tcpBuffer.Length, OnTcpRead, null);
            _udpClient.Client.BeginReceiveFrom(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, ref RemoteAddress2, OnUdpRead, null);

            _tcpWorker = new Thread(TcpWorkerLoop) { IsBackground = true };
            _tcpWorker.Start();

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


        public void FlushQueue()
        {
            _flushing = true;
            _tcpWorkerResetEvent.Set();
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
                lock (_batchedTcpLock)
                {
                    _batchingTcpQueue.Enqueue(
                        new SendableMessage {
                            Message = message,
                            MessageId = MessageInfo<T>.NextMessageId(),
                            MessageType = MessageInfo<T>.Index,
                            Serialize = MessageInfo<T>.SerializeBase
                        });
                }
            }
        }


        public void Request<TRequest>(TRequest request) where TRequest : NetworkRequest
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");


            if(MessageInfo<TRequest>.IsUdp)
                throw new InvalidOperationException();

            lock (_immediateLock)
            {
                _immediateTcpQueue.Enqueue(new SendableMessage {
                    Message = request,
                    MessageId = MessageInfo<TRequest>.NextMessageId(),
                    MessageType = MessageInfo<TRequest>.Index,
                    Serialize = MessageInfo<TRequest>.SerializeBase
                });
            }

            _tcpWorkerResetEvent.Set();
        }

        private async Task Connect(IPEndPoint endPoint)
        {
            if (IsConnected || !CanConnect)
                throw new InvalidOperationException("Client is connected");

            RemoteAddress = endPoint.Address;
            RemoteAddress2 = new IPEndPoint(endPoint.Address, sendPort);

            await _tcpClient.ConnectAsync(endPoint.Address, endPoint.Port);
            stream = _tcpClient.GetStream();

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

            _tcpClient.Close();
        }

        void IOutgoingMessageQueue.Enqueue<T>(T msg)
        {
            Enqueue(msg);
        }
    }
}