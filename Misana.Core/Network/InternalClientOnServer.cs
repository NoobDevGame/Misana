using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Misana.Core.Server;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public class InternalClientOnServer : IClientOnServer
    {
        private readonly InternalNetworkClient _serverOnClient;
        private IServerRpcMessageHandler _server;
        private readonly Queue<IGameMessage> _gameMessageQueue = new Queue<IGameMessage>();
        private readonly Queue<SendableMessage> _batchMessages = new Queue<SendableMessage>();
        private readonly Queue<SendableMessage> _msgs = new Queue<SendableMessage>();
        private readonly object _gameMessageLock = new object();
        private readonly object _batchLock = new object();
        private readonly object _msgsLock = new object();
        private readonly object _receiveLock = new object();
        private int _receiveIndex;
        private byte[] _receiveBuffer = new byte[NetworkHelper.InitialTcpBufferSize];

        private Thread _senderWorker;
        private AutoResetEvent _senderResetEvent = new AutoResetEvent(false);

        public InternalClientOnServer(InternalNetworkClient serverOnClient, IServerRpcMessageHandler handler)
        {
            _serverOnClient = serverOnClient;
            _server = handler;
        }


        public void Enqueue<T>(T msg) where T : NetworkMessage, IGameMessage
        {
            lock (_batchLock)
            {
                _batchMessages.Enqueue(new SendableMessage {
                    Message = msg,
                    MessageId = MessageInfo<T>.NextMessageId(),
                    MessageType = MessageInfo<T>.Index,
                    Serialize = MessageInfo<T>.SerializeBase
                });
            }

        }

        public int NetworkId { get; }
        public EndPoint UdpEndpoint { get { throw new NotSupportedException(); } }
        public void Respond<T>(T response, byte messageId) where T : NetworkResponse
        {
            lock (_msgsLock)
            {
                _msgs.Enqueue(new SendableMessage {
                    Message = response,
                    MessageId = messageId,
                    MessageType = MessageInfo<T>.Index,
                    Serialize = MessageInfo<T>.SerializeBase
                });
            }

            _senderResetEvent.Set();
        }

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

        public void FlushQueue()
        {
            _flushing = true;
            _senderResetEvent.Set();
        }

        public void Start()
        {
            _senderWorker = new Thread(SenderLoop) {IsBackground = true};
            _senderWorker.Start();
        }

        private bool _flushing;

        private void SenderLoop()
        {
            while (true)
            {
                _senderResetEvent.WaitOne();

                if (_msgs.Count > 0)
                {
                    lock (_msgsLock)
                    {
                        while (_msgs.Count > 0)
                        {
                            _serverOnClient.ServerOnClient.ReceiveFromInternal(_msgs.Dequeue());
                        }
                    }
                }

                if (_flushing )
                {
                    if (_batchMessages.Count > 0)
                    {
                        lock (_batchLock)
                        {
                            while (_batchMessages.Count > 0)
                            {
                                _serverOnClient.ServerOnClient.ReceiveFromInternal(_batchMessages.Dequeue());
                            }
                        }
                    }

                    _flushing = false;
                }
            }
        }

        public void HandleData(byte[] buffer, ref int processed)
        {
            var msgId = Deserializer.ReadByte(buffer, ref processed);
            var msgType = Deserializer.ReadInt32(buffer, ref processed);

            var msg = MessageInfo.Deserializers[msgType](buffer, ref processed);

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
            lock (_msgsLock)
            {
                _msgs.Enqueue(new SendableMessage {
                    Message = msg,
                    MessageId = MessageInfo<T>.NextMessageId(),
                    MessageType = MessageInfo<T>.Index,
                    Serialize = MessageInfo<T>.SerializeBase
                });
            }

            _senderResetEvent.Set();
        }

        public void ReceiveFromInternal(SendableMessage msg)
        {
            lock (_receiveLock)
            {
                _receiveIndex = 0;
                Serializer.EnsureSize(ref _receiveBuffer, _receiveIndex + 128);
                Serializer.WriteByte(msg.MessageId, ref _receiveBuffer, ref _receiveIndex);
                Serializer.WriteInt32(msg.MessageType, ref _receiveBuffer, ref _receiveIndex);
                msg.Serialize(msg.Message, ref _receiveBuffer, ref _receiveIndex);

                int pos = 0;
                HandleData(_receiveBuffer, ref pos);
            }
        }
    }
}