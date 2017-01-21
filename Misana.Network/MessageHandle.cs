using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Misana.Network
{
    internal abstract class MessageHandle
    {
        private static int maxIndex = 0;

        private static MessageIdPair[] SystemPairs;
        private static MessageIdPair[] CommunictaionPairs;

        private static readonly int headerSize = Marshal.SizeOf(typeof(MessageHeader));

        static MessageHandle()
        {
            SystemPairs = new MessageIdPair[ushort.MaxValue+1];
            CommunictaionPairs = new MessageIdPair[ushort.MaxValue+1];
        }

        protected static int RegisterType<T>()
            where T : struct
        {
            var type = typeof(T);

            var attribute = type.GetCustomAttribute<MessageDefinitionAttribute>();

            if (attribute == null)
                throw new ArgumentException($"{nameof(MessageDefinitionAttribute)} is not implemented");

            var systemId =  Interlocked.Increment(ref maxIndex);

            SystemPairs[systemId] = CommunictaionPairs[attribute.CommunictaionId] = new MessageIdPair<T>(systemId,attribute.CommunictaionId);


            return systemId;
        }

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
            Index = RegisterType<T>();
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