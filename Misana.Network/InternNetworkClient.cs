using System;
using System.Collections.Generic;

namespace Misana.Network
{
    public class InternNetworkClient : INetworkClient
    {
        public InternNetworkClient OuterClient { get; private set; }

        private MessageHandle[] handles = new MessageHandle[1];

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

        private void ReceiveMessage<T>( T message)
            where T : struct
        {
            var index = MessageHandle<T>.Index;
            if (index >= handles.Length)
                Array.Resize(ref handles,handles.Length * 2);

            MessageHandle<T> handler = (MessageHandle<T>)handles[index];

            if (handles[index] == null)
                handles[index] = handler = new MessageHandle<T>();

            handler.SetMessage(message);


        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            OuterClient.ReceiveMessage(message);
        }

        public bool TryGetMessage<T>(out T? message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            if (index >= handles.Length)
            {
                message = null;
                return false;
            }

            var handler = handles[index] as MessageHandle<T>;
            if (handler == null)
            {
                message = null;
                return false;
            }

            return handler.TryGetValue(out message);
        }
    }
}