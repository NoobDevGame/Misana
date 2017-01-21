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

        public static int RegisterType<T>()
            where T : struct
        {
            return RegisterType(typeof(T));
        }

        public static int RegisterType(Type type)
        {

            if (SystemPairs.ContainsKey(type))
                throw new ArgumentException("SystemId already");

            var systemId =  Interlocked.Increment(ref maxIndex) -1;

            SystemPairs.Add(type, new MessageIdPair(systemId,type));


            return systemId;
        }


        public static int RegisterType<T>(int index)
            where T : struct
        {
            return RegisterType(typeof(T),index);
        }

        public static int RegisterType(Type type,int index)
        {

            if (SystemPairs.ContainsKey(type))
                throw new ArgumentException("SystemId already");

            SystemPairs.Add(type, new MessageIdPair(index,type));

            if (maxIndex < index)
                maxIndex = index;

            return index;
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

        public static byte[] Serialize<T>(MessageHeader header,ref T data)
            where T : struct
        {
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

        public static T Deserialize<T>(ref byte[] data,out MessageHeader header)
            where T : struct
        {
            header = DeserializeHeader(ref data);

            return DeserializeData<T>(ref data);
        }

        public static MessageHeader DeserializeHeader(ref byte[] data)
        {

            IntPtr ptr = Marshal.AllocHGlobal(headerSize);

            Marshal.Copy(data, 0, ptr, headerSize);


            var header = Marshal.PtrToStructure<MessageHeader>(ptr);
            Marshal.FreeHGlobal(ptr);

            return header;
        }

        public static T DeserializeData<T>(ref byte[] data)
        {
            return (T)DeserializeData(ref data, typeof(T));
        }

        public static object DeserializeData(ref byte[] data,Type type)
        {
            int dataSize = Marshal.SizeOf(type);

            IntPtr ptr = Marshal.AllocHGlobal(dataSize);

            Marshal.Copy(data,headerSize, ptr, dataSize);


            var result  = Marshal.PtrToStructure(ptr,type);
            Marshal.FreeHGlobal(ptr);

            return result;
        }

        public abstract object Derserialize(ref byte[] data);

        public abstract void SetMessage(object value);

        public abstract bool TryGetValue(out object message);

        public static MessageHandle[] CreateHandleArray()
        {
            MessageHandle[] array = new MessageHandle[SystemPairs.Count];
            int i = 0;
            foreach (var pair in SystemPairs)
            {
                array[i++] = new VirtualMessageHandle(pair.Value.MessageType,pair.Value.SystemId);
            }

            return array;
        }
    }

    internal sealed class VirtualMessageHandle : MessageHandle
    {
        public readonly int Index;
        public readonly Type Type;

        private Queue<object> messages = new Queue<object>();
        private object messagesLockObject = new object();

        public VirtualMessageHandle(Type type,int index)
        {
            Type = type;
            Index = index;
        }

        public override object Derserialize(ref byte[] data)
        {
            return DeserializeData(ref data, Type);
        }

        public override void SetMessage(object message)
        {
            lock (messagesLockObject)
            {
                messages.Enqueue(message);
            }
        }

        public override bool TryGetValue(out object message)
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

    internal sealed class MessageHandle<T> : MessageHandle
        where T : struct
    {
        private static int? index;

        public static int? Index
        {
            get
            {
                if (!index.HasValue)
                {
                    index = MessageHandle.GetId<T>();
                }
                return index;
            }
        }

        private Queue<T> messages = new Queue<T>();
        private object messagesLockObject = new object();

        public static byte[] Serialize(MessageInformation information, ref T data)
        {
            return MessageHandle.Serialize<T>(new MessageHeader(information,Index.Value),ref data);
        }

        public static T Deserialize(ref byte[] data)
        {
            return MessageHandle.DeserializeData<T>(ref data);
        }

        public override object Derserialize(ref byte[] data)
        {
            return Deserialize(ref data);
        }

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

        public override void SetMessage(object value)
        {
            SetMessage((T)value);
        }

        public override bool TryGetValue(out object message)
        {
            return TryGetValue(out message);
        }
    }
}