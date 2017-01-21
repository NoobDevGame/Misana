using System;
using System.Collections.Generic;
using Misana.Network.Messages;

namespace Misana.Network
{
    public class InternNetworkClient : INetworkClient
    {
        public InternNetworkClient OuterClient { get; private set; }

        private HandleList handles = new HandleList();

        Dictionary<Type,Action<object>> callbacks = new Dictionary<Type, Action<object>>();

        private string name;

        public InternNetworkClient()
        {
            name = "client";
            OuterClient = new InternNetworkClient(this);
            Initialize();
        }

        private InternNetworkClient(InternNetworkClient outerClient)
        {
            name = "server";
            OuterClient = outerClient;
            Initialize();
        }

        private void Initialize()
        {
            RegisterOnMessageCallback<GetMessageIdMessageRequest>(ReceiveGetMessageIdRequest);
            RegisterOnMessageCallback<GetMessageIdMessageResponse>(ReceiveGetMessageIdResponse);

        }

        private void ReceiveGetMessageIdResponse(GetMessageIdMessageResponse message)
        {
            if (message.Result)
            {
                var type = Type.GetType(message.TypeName);

                var readId = MessageHandleManager.GetId(type);
                if (!readId.HasValue)
                {
                    MessageHandleManager.RegisterType(type, message.TypeId);
                }
                else if (readId.Value != message.TypeId)
                {
                    throw new Exception();
                }

                if(!handles.ExistHandle(message.TypeId))
                    handles.CreateHandle(type);
            }
        }

        private void ReceiveGetMessageIdRequest(GetMessageIdMessageRequest message)
        {
            var type = Type.GetType(message.TypeName);
            var id = MessageHandleManager.GetId(type);

            if (!id.HasValue)
            {
                id = MessageHandleManager.RegisterType(type);
                handles.CreateHandle(type);
            }

            var responseMessage = new GetMessageIdMessageResponse(true,id.Value,message.TypeName);
            SendMessage(ref responseMessage);
        }

        private void ReceiveData(byte[] data)
        {
            var header = MessageHandle.DeserializeHeader(ref data);
            var index = header.MessageIndex;

            if (!handles.ExistHandle(index))
            {
                return;
            }

            var handle = handles.GetHandle(index);
            var message = handle.Derserialize(ref data);

            if (!OnMessageReceived(handle,header,message))
                handle.SetMessage(message);



        }

        public void RegisterOnMessageCallback<T>(Action<T> callback)
            where T : struct
        {
            Action<object> objectCallback = (o) => callback((T) o);

            if (!callbacks.ContainsKey(typeof(T)))
            {
                callbacks.Add(typeof(T),objectCallback);
            }

            callbacks[typeof(T)] += objectCallback;
        }

        private bool OnMessageReceived(MessageHandle handle,MessageHeader header,object message)
        {
            if (callbacks.ContainsKey(handle.Type))
            {
                callbacks[handle.Type](message);
                return true;
            }

            return false;
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            if (!index.HasValue || !handles.ExistHandle(index.Value))
            {
                GetMessageIdMessageRequest request = new GetMessageIdMessageRequest(typeof(T));
                SendMessage<GetMessageIdMessageRequest>(ref request);
                return;
            }

            var data = MessageHandle<T>.Serialize(new MessageInformation(), ref message);
            OuterClient.ReceiveData(data);
        }

        public bool TryGetMessage<T>(out T? message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            if (!index.HasValue || !handles.ExistHandle(index.Value))
            {
                GetMessageIdMessageRequest request = new GetMessageIdMessageRequest(typeof(T));
                SendMessage<GetMessageIdMessageRequest>(ref request);
                message = null;
                return false;
            }

            var handler = handles.GetHandle(index.Value);
            if (handler == null)
            {
                message = null;
                return false;
            }

            object objMessage = null;
            var result = handler.TryGetValue(out objMessage);

            message = (T?) objMessage;

            return result;
        }
    }
}