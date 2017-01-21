using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Misana.Network
{
    internal abstract class MessageHandle
    {
        protected static int maxIndex = 0;



        public byte[] Serialize<T>(MessageHeader header,ref T data)
            where T : struct
        {
            int headerSize = Marshal.SizeOf(typeof(MessageHeader));
            int dataSize = Marshal.SizeOf(typeof(T));

            byte[] arr = new byte[dataSize + headerSize];

            {
                IntPtr ptr = Marshal.AllocHGlobal(headerSize);
                Marshal.StructureToPtr(header, ptr, true);
                Marshal.Copy(ptr, arr, 0, headerSize);
                Marshal.FreeHGlobal(ptr);
            }

            {
                IntPtr ptr = Marshal.AllocHGlobal(dataSize);
                Marshal.StructureToPtr(data, ptr, true);
                Marshal.Copy(ptr, arr, headerSize, dataSize);
                Marshal.FreeHGlobal(ptr);
            }
            return arr;
        }

        public T Deserialize<T>(byte[] data,out MessageHeader header)
            where T : struct
        {
            int headerSize = Marshal.SizeOf(typeof(MessageHeader));
            int dataSize = Marshal.SizeOf(typeof(T));

            {
                IntPtr ptr = Marshal.AllocHGlobal(headerSize);

                Marshal.Copy(data,0, ptr, headerSize);


                header = Marshal.PtrToStructure<MessageHeader>(ptr);
                Marshal.FreeHGlobal(ptr);
            }

            {
                IntPtr ptr = Marshal.AllocHGlobal(dataSize);

                Marshal.Copy(data,headerSize, ptr, dataSize);


                var result  = Marshal.PtrToStructure<T>(ptr);
                Marshal.FreeHGlobal(ptr);

                return result;
            }

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

        public byte[] Serialize(MessageHeader header,ref T data) => Serialize<T>(header, ref data);

        public T Deserialize(byte[] data,out MessageHeader header) => Deserialize<T>(data,out header);

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