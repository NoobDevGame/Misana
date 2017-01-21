using System;
using System.Collections.Generic;

namespace Misana.Network
{
    public class InternNetworkClient : INetworkClient
    {
        public InternNetworkClient OuterClient { get; private set; }

        private HandleList handles = new HandleList();

        Dictionary<Type,Action<object>> callbacks = new Dictionary<Type, Action<object>>();

        private string name;

        public InternNetworkClient()
        {
            name = "client";
            OuterClient = new InternNetworkClient(this);
            Initialize();
        }

        private InternNetworkClient(InternNetworkClient outerClient)
        {
            name = "server";
            OuterClient = outerClient;
            Initialize();
        }

        private void Initialize()
        {

        }



        private void ReceiveData(byte[] data)
        {
            var header = MessageHandle.DeserializeHeader(ref data);
            var index = header.MessageIndex;

            if (!handles.ExistHandle(index))
            {
                return;
            }

            var handle = handles.GetHandle(index);
            var message = handle.Derserialize(ref data);

            if (!OnMessageReceived(handle,header,message))
                handle.SetMessage(message);



        }

        public void RegisterOnMessageCallback<T>(Action<T> callback)
            where T : struct
        {
            Action<object> objectCallback = (o) => callback((T) o);

            if (!callbacks.ContainsKey(typeof(T)))
            {
                callbacks.Add(typeof(T),objectCallback);
            }

            callbacks[typeof(T)] += objectCallback;
        }

        private bool OnMessageReceived(MessageHandle handle,MessageHeader header,object message)
        {
            if (callbacks.ContainsKey(handle.Type))
            {
                callbacks[handle.Type](message);
                return true;
            }

            return false;
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            var data = MessageHandle<T>.Serialize(new MessageInformation(), ref message);
            OuterClient.ReceiveData(data);
        }

        public bool TryGetMessage<T>(out T? message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            var handler = handles.GetHandle(index.Value);
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
    }
}