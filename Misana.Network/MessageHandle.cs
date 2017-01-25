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
        private static readonly int headerSize = Marshal.SizeOf(typeof(MessageHeader));
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

        public abstract void SetMessage(object value,MessageHeader header,NetworkClient client);

        public abstract bool TryGetValue(out object message, out INetworkClient senderClient);

        public abstract void SetCallbackHandles(ref MessageWaitObject[] waitObjects);
    }

    internal sealed class MessageHandle<T> : MessageHandle
        where T : struct
    {
        private class MessageInfo
        {
            public T Message { get; }
            public INetworkClient Client { get; }


            public MessageInfo(T message,INetworkClient client)
            {
                Message = message;
                Client = client;
            }
        }

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

        private Queue<MessageInfo> messages = new Queue<MessageInfo>();
        private object messagesLockObject = new object();

        private MessageReceiveCallback<T> _callback;

        private static MessageWaitObject[] receiveWaitHandles;
        private static MessageWaitObject[] sendWaitHandles;

        public static bool IsResponse { get;  private set; }

        public MessageHandle()
            : base(typeof(T))
        {

        }

        public override void Initialize(MessageDefinitionAttribute attribute)
        {
            IsResponse = attribute.IsResponse;

            if (attribute.ResponseType != null)
            {
                var handler = MessageHandleManager.CreateMessageHandle(attribute.ResponseType);
                handler.SetCallbackHandles(ref sendWaitHandles);
            }
        }

        public static byte[] Serialize(ref T data,out MessageWaitObject waitObject)
        {
            if (IsResponse)
                throw new NotSupportedException("For Requestmessages only");

            var messageId = (byte)(Interlocked.Increment(ref MessageId) % byte.MaxValue);

            waitObject = sendWaitHandles?[messageId];

            return MessageHandle.Serialize<T>(new MessageHeader(Index.Value,messageId),ref data);
        }

        public static byte[] Serialize(ref T data,byte messageId)
        {
            if (!IsResponse)
                throw new NotSupportedException("For Responsemessages only");

            return MessageHandle.Serialize<T>(new MessageHeader(Index.Value,messageId),ref data);
        }

        public override void SetCallbackHandles(ref MessageWaitObject[] waitObjects)
        {
            if (receiveWaitHandles == null)
            {
                receiveWaitHandles = new MessageWaitObject[byte.MaxValue+1];
                for (int i = 0; i < byte.MaxValue +1; i++)
                    receiveWaitHandles[i] = new MessageWaitObject();
            }

            waitObjects = receiveWaitHandles;
        }

        public static T Deserialize(ref byte[] data)
        {
            return DeserializeData<T>(ref data);
        }

        public override object Derserialize(ref byte[] data)
        {
            return Deserialize(ref data);
        }

        public bool TryGetValue(out T? message,out INetworkClient senderClient)
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
            message = null;
            return false;
        }

        public override bool TryGetValue(out object message, out INetworkClient senderClient)
        {
            T? value;
            var result = TryGetValue(out value,out senderClient);

            message = value;

            return result;
        }

        public void SetMessage(T message,MessageHeader header,NetworkClient client)
        {
            receiveWaitHandles?[header.MessageId].Release(message);

            if (_callback != null)
            {
                _callback.Invoke(message,header,client);
                return;
            }

            lock (messagesLockObject)
            {
                messages.Enqueue(new MessageInfo(message,client));
            }
        }

        public override void SetMessage(object value,MessageHeader header,NetworkClient client)
        {
            SetMessage((T)value,header,client);
        }

        public void RegisterCallback(MessageReceiveCallback<T> callback)
        {
            _callback += callback;
        }
    }
}