using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Misana.Network
{
    internal abstract class MessageHandle
    {
        private static readonly int HeaderSize = Marshal.SizeOf(typeof(MessageHeader));
        public readonly Type Type;

        protected MessageHandle(Type type)
        {
            Type = type;
        }

        public abstract void Initialize(MessageDefinitionAttribute attribute);

        public static byte[] Serialize<T>(MessageHeader header,ref T data)
            where T : struct
        {
            int dataSize = Marshal.SizeOf(typeof(T));

            byte[] arr = new byte[dataSize + HeaderSize];

            {
                int actual;
                var ptr = BlockAllocator.Alloc(HeaderSize, out actual);
                Marshal.StructureToPtr(header, ptr, true);
                Marshal.Copy(ptr, arr, 0, HeaderSize);
                BlockAllocator.Free(ptr, actual);
            }

            {
                int actual;
                var ptr = BlockAllocator.Alloc(dataSize, out actual);
                Marshal.StructureToPtr(data, ptr, true);
                Marshal.Copy(ptr, arr, HeaderSize, dataSize);
                BlockAllocator.Free(ptr, actual);
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
            int actual;
            var ptr = BlockAllocator.Alloc(HeaderSize, out actual);

            Marshal.Copy(data, 0, ptr, HeaderSize);

            var header = Marshal.PtrToStructure<MessageHeader>(ptr);
            BlockAllocator.Free(ptr, actual);

            return header;
        }

        public static T DeserializeData<T>(ref byte[] data)
        {
            return (T)DeserializeData(ref data, typeof(T));
        }

        public static object DeserializeData(ref byte[] data,Type type)
        {
            int dataSize = Marshal.SizeOf(type);

            int actual;
            var ptr = BlockAllocator.Alloc(dataSize, out actual);

            Marshal.Copy(data,HeaderSize, ptr, dataSize);


            var result  = Marshal.PtrToStructure(ptr,type);
            BlockAllocator.Free(ptr, actual);

            return result;
        }

        public abstract object Derserialize(ref byte[] data);

        public abstract void SetCallbackHandles(out MessageWaitObject[] waitObjects);

        public abstract void SetMessage(object message, MessageHeader header, INetworkClient networkClient);
    }

    internal sealed partial class MessageHandle<T> : MessageHandle
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

        public static int MessageId;

        private class MessageInfo
        {
            public T Message { get; }
            public INetworkIdentifier Client { get; }


            public MessageInfo(T message,INetworkIdentifier client)
            {
                Message = message;
                Client = client;
            }
        }

        private Queue<MessageInfo> messages = new Queue<MessageInfo>();
        private object messagesLockObject = new object();

        private MessageReceiveCallback<T> _callback;

        private static MessageWaitObject[] receiveWaitHandles;
        private static MessageWaitObject[] sendWaitHandles;

        public static bool IsResponse { get;  private set; }
        public static bool IsUDPMessage { get; private set; }
        public static bool NoQueue { get; private set; }

        public MessageHandle()
            : base(typeof(T))
        {

        }

        public override void Initialize(MessageDefinitionAttribute attribute)
        {
            IsResponse = attribute.IsResponse;
            IsUDPMessage = attribute.UseUDP;
            NoQueue = attribute.NoQueue;

            if (attribute.ResponseType != null)
            {
                var handler = MessageHandleManager.CreateMessageHandle(attribute.ResponseType);
                handler.SetCallbackHandles(out sendWaitHandles);
            }
        }

        public override void SetCallbackHandles(out MessageWaitObject[] waitObjects)
        {
            if (receiveWaitHandles == null)
            {
                receiveWaitHandles = new MessageWaitObject[byte.MaxValue+1];
                for (int i = 0; i < byte.MaxValue +1; i++)
                    receiveWaitHandles[i] = new MessageWaitObject();
            }

            waitObjects = receiveWaitHandles;
        }


        public static byte[] Serialize(ref T data,out MessageWaitObject waitObject)
        {
            if (IsResponse)
                throw new NotSupportedException("For Requestmessages only");

            var messageId = GetWaitObject(out waitObject);

            return MessageHandle.Serialize<T>(new MessageHeader(Index.Value,messageId),ref data);
        }

        public static byte GetWaitObject(out MessageWaitObject waitObject)
        {
            var messageId = (byte)(Interlocked.Increment(ref MessageId) % byte.MaxValue);
            waitObject = sendWaitHandles?[messageId];

            return messageId;
        }

        public static byte[] Serialize(ref T data,byte messageId)
        {
            if (!IsResponse)
                throw new NotSupportedException("For Responsemessages only");

            return MessageHandle.Serialize<T>(new MessageHeader(Index.Value,messageId),ref data);
        }


        public static T Deserialize(ref byte[] data)
        {
            return DeserializeData<T>(ref data);
        }

        public override object Derserialize(ref byte[] data)
        {
            return Deserialize(ref data);
        }

        public bool TryGetValue(out T message,out INetworkIdentifier senderClient)
        {
            if (messages.Count > 0)
            {
                MessageInfo info = null;
                lock (messagesLockObject)
                {
                    info = messages.Dequeue();

                }

                message = info.Message;
                senderClient = info.Client;

                return true;
            }


            senderClient = null;
            message = default(T);
            return false;
        }

        public void SetMessage(T message,MessageHeader header,INetworkClient client)
        {
            receiveWaitHandles?[header.MessageId].Release(message);

            _callback?.Invoke(message,header,client);

            if (!NoQueue)
            {
                lock (messagesLockObject)
                {
                    messages.Enqueue(new MessageInfo(message,client));
                }
            }

        }

        public override void SetMessage(object message, MessageHeader header, INetworkClient networkClient)
        {
            SetMessage((T)message,header,networkClient);
        }

        public void RegisterCallback(MessageReceiveCallback<T> callback)
        {
            _callback += callback;
        }


    }
}