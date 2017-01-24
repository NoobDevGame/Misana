using System;
using System.Threading.Tasks;

namespace Misana.Network
{
    public class NetworkClient
    {

        public NetworkClient Server { get; private set; }

        private MessageHandleList _messageHandles = new MessageHandleList();

        public bool IsConnected { get; private set; }

        private string name;

        public NetworkClient()
        {
            name = "client";
            Server = new NetworkClient(this);
        }

        private NetworkClient(NetworkClient server)
        {
            name = "server";
            Server = server;
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

            handle.SetMessage(message,header);
        }

        public void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback)
            where T : struct
        {
            _messageHandles.GetHandle<T>().RegisterCallback(callback);
        }

        public MessageWaitObject SendMessage<T>(ref T message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            MessageWaitObject waitObject = null;

            var data = MessageHandle<T>.Serialize(ref message,out waitObject);

            waitObject?.Start();

            Server.ReceiveData(data);

            return waitObject;
        }

        public void SendMessage<T>(ref T message,byte messageid) where T : struct
        {
            var index = MessageHandle<T>.Index;

            var data = MessageHandle<T>.Serialize(ref message,messageid);

            Server.ReceiveData(data);
        }

        public bool TryGetMessage<T>(out T? message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            var handler = _messageHandles.GetHandle(index.Value);
            if (handler == null)
            {
                message = null;
                return false;
            }

            object objMessage = null;
            var result = handler.TryGetValue(out objMessage);

            message = (T?) objMessage;

            return result;
        }

        public async Task Connect()
        {
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }

    }
}