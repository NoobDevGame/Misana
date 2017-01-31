using System;
using System.Net;
using System.Threading.Tasks;

namespace Misana.Network
{
    public class InternalNetworkClient : INetworkClient
    {
        public int NetworkId { get; } = NetworkManager.GetNextId();

        public bool IsConnected { get; } = true;
        public bool CanSend { get; } = true;

        public INetworkClient ServerClient { get; }
        public INetworkClient LocalClient { get; }

        private readonly MessageHandleList _messageHandles = new MessageHandleList();

        private readonly InternalNetworkClient _outer;

        private readonly string name;
        public InternalNetworkClient()
        {
            _outer = new InternalNetworkClient(this);
            name = "Client";

            LocalClient = this;
            ServerClient = _outer;

        }

        private InternalNetworkClient(InternalNetworkClient client)
        {
            _outer = client;
            name = "Server";

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

        public void SendTcpBytes(byte[] bytes)
        {
            _outer.ReceiveData(bytes);
        }

        public void SendUdpBytes(byte[] bytes)
        {
            _outer.ReceiveData(bytes);
        }

        public void ReceiveData(byte[] data)
        {
            var header = MessageHandle.DeserializeHeader(ref data);
            var index = header.MessageTypeIndex;

            if (!_messageHandles.ExistHandle(index))
            {
                return;
            }

            var handle = _messageHandles.GetHandle(index);
            var message = handle.Derserialize(ref data);

            handle.SetMessage(message, header, this);
        }

        public MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct
        {
            MessageWaitObject waitObject = null;

            var messageId = MessageHandle<T>.GetWaitObject(out waitObject);

            waitObject?.Start();

            _outer.Receive(ref message,messageId);

            return waitObject;
        }

        private void Receive<T>(ref T message,byte id)
            where T : struct
        {
            var header = new MessageHeader(MessageHandle<T>.Index.Value,id);
            _messageHandles.GetHandle<T>().SetMessage(message,header, this);
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