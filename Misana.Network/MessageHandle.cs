using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Misana.Network
{
    internal abstract class MessageHandle
    {
        protected static int maxIndex = 0;

        public byte[] Serialize<T>(MessageHeaderState state,ref T data)
            where T : struct
        {
            int size = Marshal.SizeOf(typeof(T)) + sizeof(MessageHeaderState);
            byte[] arr = new byte[size];
            arr[0] = (byte) state;


            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, ptr, true);
            Marshal.Copy(ptr, arr, sizeof(MessageHeaderState), size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public T Deserialize<T>(byte[] data,out MessageHeaderState state)
            where T : struct
        {
            int size = Marshal.SizeOf(typeof(T)) + sizeof(MessageHeaderState) ;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(data, sizeof(MessageHeaderState), ptr, size);


            var value = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);

            state = (MessageHeaderState) data[0];

            return value;
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

        public byte[] Serialize(MessageHeaderState state,ref T data) => Serialize<T>(state, ref data);

        public T Deserialize(byte[] data,out MessageHeaderState state) => Deserialize<T>(data,out state);

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