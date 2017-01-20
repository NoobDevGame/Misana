using System;
using System.Collections.Generic;

namespace Misana.Network
{
    public class InternNetworkClient : INetworkClient
    {
        public InternNetworkClient OuterClient { get; private set; }

        private Dictionary<Type,Queue<object>> _messages = new Dictionary<Type, Queue<object>>();

        private string name;

        public InternNetworkClient()
        {
            name = "client";
            OuterClient = new InternNetworkClient(this);
        }

        private InternNetworkClient(InternNetworkClient outerClient)
        {
            name = "server";
            OuterClient = outerClient;
        }

        private void ReceiveMessageFast<T>(uint messageType, T message)
            where T : struct
        {
            Queue<object> messages = null;

            if (!_messages.TryGetValue(typeof(T),out messages))
            {
                messages = new Queue<object>();
                _messages.Add(typeof(T),messages);
            }

            messages.Enqueue(message);
        }

        public void SendMessageFast<T>(uint messageType, ref T message) where T : struct
        {
            OuterClient.ReceiveMessageFast(messageType,message);
        }

        public bool TryGetMessage<T>(uint messageType, out T? message) where T : struct
        {
            Queue<object> messages = null;

            if (_messages.TryGetValue(typeof(T),out messages) && messages.Count > 0 )
            {
                message = (T)messages.Dequeue();
                return true;
            }

            message = null;
            return false;
        }
    }
}