using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Misana.Network
{
    public class NetworkClient : INetworkSender, INetworkReceiver, INetworkClient
    {
        private readonly NetworkListener _listener;
        private NetworkClient _outerClient;

        private static int clientId = 0;
        public int ClientId { get; } = Interlocked.Increment(ref clientId);


        private MessageHandleList _messageHandles = new MessageHandleList();

        public bool IsConnected { get; private set; }

        internal NetworkClient(NetworkListener listener)
        {
            _listener = listener;
        }

        internal NetworkClient(NetworkClient client)
        {
            IsConnected = true;
            _outerClient = client;
        }

        private void ReceiveData(byte[] data)
        {
            var header = MessageHandle.DeserializeHeader(ref data);
            var index = header.MessageTypeIndex;

            if (!_messageHandles.ExistHandle(index))
            {
                return;
            }

            var handle = _messageHandles.GetHandle(index);
            var message = handle.Derserialize(ref data);

            handle.SetMessage(message,header,this);
        }

        public void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback)
            where T : struct
        {
            _messageHandles.GetHandle<T>().RegisterCallback(callback);
        }

        public MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            var index = MessageHandle<T>.Index;

            MessageWaitObject waitObject = null;

            var data = MessageHandle<T>.Serialize(ref message,out waitObject);

            waitObject?.Start();

            _outerClient.ReceiveData(data);

            return waitObject;
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            SendRequestMessage(ref message);
        }

        public void SendResponseMessage<T>(ref T message,byte messageid) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            var index = MessageHandle<T>.Index;

            var data = MessageHandle<T>.Serialize(ref message,messageid);

            _outerClient.ReceiveData(data);
        }

        public bool TryGetMessage<T>(out T message, out INetworkClient senderClient)
            where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

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
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            INetworkClient client;
            return TryGetMessage(out message, out client);
        }

        public async Task Connect()
        {
            if (IsConnected)
                throw new InvalidOperationException("Client is connected");

            _outerClient = _listener.CreateClient(this);

            IsConnected = true;
        }

        public void Disconnect()
        {
            if (false && !IsConnected)
                throw new InvalidOperationException("Client is not connected");


            IsConnected = false;

            _outerClient.Disconnect();
            _outerClient = null;
        }



    }
}