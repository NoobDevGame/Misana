using System;
using System.Net;
using System.Threading.Tasks;

namespace Misana.Network
{
    public class InternalNetworkClient : INetworkClient
    {
        public int NetworkId { get; } = NetworkManager.GetNextId();

        public bool IsConnected { get; } = true;

        public INetworkClient ServerClient { get; }
        public INetworkClient LocalClient { get; }

        private readonly MessageHandleList _messageHandles = new MessageHandleList();

        private readonly InternalNetworkClient _outer;

        public InternalNetworkClient()
        {
            _outer = new InternalNetworkClient(this);

            LocalClient = this;
            ServerClient = _outer;

        }

        public InternalNetworkClient(InternalNetworkClient client)
        {
            _outer = client;

            LocalClient = client;
            ServerClient = this;
        }

        public async Task Connect(IPAddress addr)
        {
        }

        public void Disconnect()
        {
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            SendRequestMessage(ref message);
        }

        public MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct
        {
            MessageWaitObject waitObject = null;

            MessageHandle<T>.GetWaitObject(out waitObject);

            waitObject?.Start();

            _outer.Receive(ref message);

            return waitObject;
        }

        private void Receive<T>(ref T message)
            where T : struct
        {
            var header = new MessageHeader(MessageHandle<T>.Index.Value);
            _messageHandles.GetHandle<T>().SetMessage(message,default(MessageHeader), _outer);
        }

        private void Receive<T>(ref T message,byte id)
            where T : struct
        {
            var header = new MessageHeader(MessageHandle<T>.Index.Value,id);
            _messageHandles.GetHandle<T>().SetMessage(message,header, _outer);
        }

        public void SendResponseMessage<T>(ref T message, byte messageid) where T : struct
        {
            _outer.Receive(ref message,messageid);
        }

        public bool TryGetMessage<T>(out T message, out INetworkIdentifier senderClient) where T : struct
        {
            var index = MessageHandle<T>.Index;

            var handler = (MessageHandle<T>)_messageHandles.GetHandle(index.Value);
            if (handler == null)
            {
                senderClient = null;
                message = default(T);
                return false;
            }

            return handler.TryGetValue(out message,out senderClient);
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            INetworkIdentifier client;
            return TryGetMessage(out message, out client);
        }



        public void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback) where T : struct
        {
            _messageHandles.GetHandle<T>().RegisterCallback(callback);
        }







    }
}