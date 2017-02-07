using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Misana.Core.Client;
using Misana.Core.Server;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public partial class ClientOnServer : IClientOnServer
    {
        private readonly ServerGameHost _server;
        public int NetworkId { get; } = NetworkManager.GetNextId();
        public EndPoint UdpEndpoint { get; }
        public IClientRpcMessageHandler ClientRpcHandler { get; set; }

        private TcpHandler _tcpHandler;

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

        internal ClientOnServer(ServerGameHost server, TcpClient tcpClient, IPAddress address)
        {
            _server = server;
            RemoteAddress = address;
            _tcpHandler = new TcpHandler(tcpClient, HandleData, OnDisconnect);
            UdpEndpoint = new IPEndPoint(address, NetworkManager.LocalUdpPort);


        }

        private readonly Queue<IGameMessage> _gameMessageQueue = new Queue<IGameMessage>();
        private readonly object _gameMessageLock = new object();

        public void Start()
        {
            _tcpHandler.Start();
        }

        public void HandleData(byte[] data, ref int processed)
        {
            var msgId = Deserializer.ReadByte(data, ref processed);
            var msgType = Deserializer.ReadInt32(data, ref processed);

            var msg = MessageInfo.Deserializers[msgType](data, ref processed);

            if (MessageInfo.IsRpcMessage[msgType])
                ((IRpcMessage) msg).HandleOnServer(_server, msgId, this);
            else
            {
                lock (_gameMessageLock)
                {
                    _gameMessageQueue.Enqueue((IGameMessage) msg);
                }
            }
        }

        public void Send<T>(T msg) where T : RpcMessage
        {
            if (MessageInfo<T>.IsUdp)
                throw new InvalidOperationException();

            _tcpHandler.Send(new SendableMessage {
                Message = msg,
                MessageId = MessageInfo<T>.NextMessageId(),
                MessageType = MessageInfo<T>.Index,
                Serialize = MessageInfo<T>.SerializeBase
            });
        }

        public void FlushQueue()
        {
            _tcpHandler.Flush();
        }

        public void Enqueue<T>(T message) where T : NetworkMessage, IGameMessage
        {
            if (MessageInfo<T>.IsUdp)
            {
                _server.EnqueueUdp(message, this);
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

        private void OnDisconnect()
        {
            _server.OnDisconnectClient(this);
        }

        public void Respond<TResponse>(TResponse request, byte messageId) where TResponse : NetworkResponse
        {
            if (MessageInfo<TResponse>.IsUdp)
                throw new InvalidOperationException();

            _tcpHandler.Send(new SendableMessage {
                Message = request,
                MessageId = messageId,
                MessageType = MessageInfo<TResponse>.Index,
                Serialize = MessageInfo<TResponse>.SerializeBase
            });
        }
    }
}