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
        private static readonly int headerSize = Marshal.SizeOf(typeof(MessageHeader));
        public readonly Type Type;

        public MessageHandle(Type type)
        {
            Type = type;
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
                    index = MessageHandleManager.GetId<T>();
                }
                return index;
            }
        }

        private Queue<T> messages = new Queue<T>();
        private object messagesLockObject = new object();

        public MessageHandle()
            : base(typeof(T))
        {

        }

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

        public override bool TryGetValue(out object message)
        {
            T? value;
            var result = TryGetValue(out value);

            message = value;

            return result;
        }

        public void SetMessage(T message)
        {
            lock (messagesLockObject)
            {
                messages.Enqueue(message);
            }
        }

        public override void SetMessage(object value)
        {
            SetMessage((T)value);
        }
    }
}