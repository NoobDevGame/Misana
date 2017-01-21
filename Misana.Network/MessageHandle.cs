using System;
using System.Collections.Generic;
using System.Threading;

namespace Misana.Network
{
    internal abstract class MessageHandle
    {
        protected static int maxIndex = 0;

        public byte[] Serialize<T>(T data)
            where T : struct
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(byte[] data)
            where T : struct
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class MessageHandle<T> : MessageHandle
        where T : struct
    {
        public static int Index = 0;

        private Queue<T> messages = new Queue<T>();
        private object messagesLockObject = new object();

        static MessageHandle()
        {
            Index = Interlocked.Increment(ref maxIndex);
        }

        public byte[] Serialize(T data) => Serialize<T>(data);

        public T Deserialize(byte[] data) => Deserialize<T>(data);

        public void SetMessage(T message)
        {
            lock (messagesLockObject)
            {
                messages.Enqueue(message);
            }
        }

        public bool TryGetValue(out T? message)
        {
            if (messages.Count > 0)
            {
                lock (messagesLockObject)
                {
                    message = messages.Dequeue();
                }

                return true;
            }

            message = null;
            return false;
        }
    }
}