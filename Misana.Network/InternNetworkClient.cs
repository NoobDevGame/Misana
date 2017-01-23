using System;
using System.Collections.Generic;

namespace Misana.Network
{
    public class InternNetworkClient : INetworkClient
    {
        public InternNetworkClient Server { get; private set; }

        private MessageHandleList _messageHandles = new MessageHandleList();

        public bool IsConnected { get; private set; }

        private string name;

        public InternNetworkClient()
        {
            name = "client";
            Server = new InternNetworkClient(this);
        }

        private InternNetworkClient(InternNetworkClient server)
        {
            name = "server";
            Server = server;
        }

        private void ReceiveData(byte[] data)
        {
            var header = MessageHandle.DeserializeHeader(ref data);
            var index = header.MessageIndex;

            if (!_messageHandles.ExistHandle(index))
            {
                return;
            }

            var handle = _messageHandles.GetHandle(index);
            var message = handle.Derserialize(ref data);

            handle.SetMessage(message);
        }

        public void RegisterOnMessageCallback<T>(Action<T> callback)
            where T : struct
        {
            _messageHandles.GetHandle<T>().RegisterCallback(callback);
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            var data = MessageHandle<T>.Serialize(new MessageInformation(), ref message);
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

        public void Connect()
        {
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }


    }
}