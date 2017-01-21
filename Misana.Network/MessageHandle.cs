using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Misana.Network.Messages;

namespace Misana.Network
{
    internal abstract class MessageHandle
    {
        private static int maxIndex = 0;

        private static Dictionary<Type,MessageIdPair> SystemPairs = new Dictionary<Type, MessageIdPair>();

        private static readonly int headerSize = Marshal.SizeOf(typeof(MessageHeader));

        static MessageHandle()
        {
            RegisterType<GetMessageIDMessageRequest>();
            RegisterType<GetMessageIDMessageResponse>();
        }

        protected static int RegisterType<T>()
            where T : struct
        {
            var type = typeof(T);

            var systemId =  Interlocked.Increment(ref maxIndex);

            if (SystemPairs.ContainsKey(type))
                throw new ArgumentException("SystemId already");

            SystemPairs.Add(type, new MessageIdPair<T>(systemId));


            return systemId;
        }

        public static int? GetId<T>()
            where T : struct
        {
            return GetId(typeof(T));
        }

        public static int? GetId(Type type)
        {
            if (SystemPairs.ContainsKey(type))
            {
                return SystemPairs[type].SystemId;
            }

            return null;
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