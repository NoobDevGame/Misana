using System;
using System.Collections.Generic;

namespace Misana.Network
{
    public class BroadcastList<T> : List<T> , IBroadcastSender , INetworkReceiver
        where T : INetworkSender , INetworkIdentifier , INetworkReceiver
    {
        private Dictionary<Type,Action<object,MessageHeader,INetworkClient>> callbacks = new Dictionary<Type, Action<object, MessageHeader, INetworkClient>>();

        public BroadcastList()
        {

        }

        public void SendMessage<T1>(ref T1 message) where T1 : struct
        {
            foreach (var item in this)
            {
                item.SendMessage(ref message);
            }
        }

        public MessageWaitObject SendRequestMessage<T1>(ref T1 message) where T1 : struct
        {
            throw new NotSupportedException();
        }

        public void SendResponseMessage<T1>(ref T1 message, byte messageid) where T1 : struct
        {
            throw new NotSupportedException();
        }

        public void SendMessage<T1>(ref T1 message, int originId) where T1 : struct
        {
            foreach (var item in this)
            {
                if (item.NetworkId == originId)
                    continue;

                item.SendMessage(ref message);
            }
        }

        private int userIndex = 0;

        public bool TryGetMessage<T1>(out T1 message, out INetworkIdentifier senderClient) where T1 : struct
        {
            for (; userIndex < Count; userIndex++)
            {
                var client = this[userIndex];
                var result = client.TryGetMessage(out message, out senderClient);

                if (result)
                {
                    return true;
                }
            }

            userIndex = 0;
            message = default(T1);
            senderClient = null;
            return false;
        }

        public bool TryGetMessage<T1>(out T1 message) where T1 : struct
        {
            for (; userIndex < Count; userIndex++)
            {
                var client = this[userIndex];
                INetworkIdentifier senderClient = null;
                var result = client.TryGetMessage(out message, out senderClient);

                if (result)
                {
                    return true;
                }
            }

            userIndex = 0;
            message = default(T1);
            return false;
        }

        public void ReceiveMessage<T1>(ref T1 message,MessageHeader header,INetworkClient client)
            where T1 : struct
        {
            if (callbacks.ContainsKey(typeof(T1)))
            {
                callbacks[typeof(T1)]?.Invoke(message,header,client);
            }
        }

        public void RegisterOnMessageCallback<T1>(MessageReceiveCallback<T1> callback) where T1 : struct
        {
            Action<object,MessageHeader,INetworkClient> action = (o, h, c) => callback?.Invoke((T1) o, h, c);

            if (callbacks.ContainsKey(typeof(T1)))
            {
                callbacks[typeof(T1)] += action;
            }
            else
            {
                callbacks.Add(typeof(T1),action);
            }


        }
    }
}