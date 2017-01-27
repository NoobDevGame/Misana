using System;
using System.Collections.Generic;

namespace Misana.Network
{
    public class BroadcastList<T> : List<T> , IBroadcastSender , INetworkReceiver
        where T : INetworkSender , INetworkClient , INetworkReceiver
    {
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
                if (item.ClientId == originId)
                    continue;

                item.SendMessage(ref message);
            }
        }

        private int userIndex = 0;

        public bool TryGetMessage<T1>(out T1 message, out INetworkClient senderClient) where T1 : struct
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
                INetworkClient senderClient = null;
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

        public void RegisterOnMessageCallback<T1>(MessageReceiveCallback<T1> callback) where T1 : struct
        {

        }
    }
}