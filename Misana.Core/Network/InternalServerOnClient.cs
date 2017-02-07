using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Misana.Core.Client;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public class InternalServerOnClient : IServerOnClient
    {
        private readonly InternalNetworkClient _clientOnServer;


        public InternalServerOnClient(InternalNetworkClient clientOnServer, IClientRpcMessageHandler handler)
        {
            _clientOnServer = clientOnServer;
            _client = handler;
        }

        private readonly Queue<SendableMessage> _batchMessages = new Queue<SendableMessage>();
        private readonly Queue<SendableMessage> _msgs = new Queue<SendableMessage>();
        private readonly object _gameMessageLock = new object();
        private readonly object _batchLock = new object();
        private readonly object _msgsLock = new object();
        private readonly object _receiveLock = new object();
        private int _receiveIndex;
        private byte[] _receiveBuffer = new byte[NetworkHelper.InitialTcpBufferSize];
        private bool _flushing;

        private Thread _senderWorker;
        private AutoResetEvent _senderResetEvent = new AutoResetEvent(false);

        public void Enqueue<T>(T msg) where T : NetworkMessage, IGameMessage
        {
            lock (_batchLock)
            {
                _batchMessages
                    .Enqueue(
                        new SendableMessage {
                            Message = msg,
                            MessageId = MessageInfo<T>.NextMessageId(),
                            MessageType = MessageInfo<T>.Index,
                            Serialize = MessageInfo<T>.SerializeBase
                        });
            }
        }

        public void FlushQueue()
        {
            _flushing = true;
            _senderResetEvent.Set();
        }

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
                            _clientOnServer.ClientOnServer.ReceiveFromInternal(_msgs.Dequeue());
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
                                _clientOnServer.ClientOnServer.ReceiveFromInternal(_batchMessages.Dequeue());
                            }
                        }
                    }

                    _flushing = false;
                }
            }
        }

        private readonly Queue<IGameMessage> _gameMessageQueue = new Queue<IGameMessage>();
        private IClientRpcMessageHandler _client;

        public void HandleData(byte[] data, ref int processed)
        {
            var msgId = Deserializer.ReadByte(data, ref processed);
            var msgType = Deserializer.ReadInt32(data, ref processed);

            var msg = MessageInfo.Deserializers[msgType](data, ref processed);

            if (MessageInfo.IsRpcMessage[msgType])
                ((IRpcMessage) msg).HandleOnClient(_client);
            else
            {
                lock (_gameMessageLock)
                {
                    _gameMessageQueue.Enqueue((IGameMessage) msg);
                }
            }
        }

        public Task Connect(IPAddress address)
        {
            _senderWorker = new Thread(SenderLoop) {IsBackground = true};
            _senderWorker.Start();
            IsConnected = true;

            return Task.CompletedTask;
        }

        public void Disconnect()
        {
            throw new System.NotImplementedException();
        }

        public bool IsConnected { get; set; }
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

        public void Request<TRequest>(TRequest request) where TRequest : NetworkRequest
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");


            if(MessageInfo<TRequest>.IsUdp)
                throw new InvalidOperationException();

            lock (_msgsLock)
            {
                _msgs.Enqueue( new SendableMessage {
                    Message = request,
                    MessageId = MessageInfo<TRequest>.NextMessageId(),
                    MessageType = MessageInfo<TRequest>.Index,
                    Serialize = MessageInfo<TRequest>.SerializeBase
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