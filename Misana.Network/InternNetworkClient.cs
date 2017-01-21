using System;
using System.Collections.Generic;
using Misana.Network.Messages;

namespace Misana.Network
{
    public class InternNetworkClient : INetworkClient
    {
        public InternNetworkClient OuterClient { get; private set; }

        private HandleList handles = new HandleList();

        private string name;

        public InternNetworkClient()
        {

            name = "client";
            OuterClient = new InternNetworkClient(this);
        }

        private InternNetworkClient(InternNetworkClient outerClient)
        {
            name = "server";
            OuterClient = outerClient;
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

        private bool OnMessageReceived(MessageHandle handle,MessageHeader header,object message)
        {
            if (MessageHandle<GetMessageIDMessageRequest>.Index != null
                && header.MessageIndex == MessageHandle<GetMessageIDMessageRequest>.Index.Value)
            {
                var castMessage = (GetMessageIDMessageRequest) message;
                var type = Type.GetType(castMessage.TypeName);
                var id = MessageHandle.GetId(type);

                if (!id.HasValue)
                {
                    id = MessageHandle.RegisterType(type);
                    handles.CreateHandle(type);
                }

                var responseMessage = new GetMessageIDMessageResponse(true,id.Value,castMessage.TypeName);
                SendMessage(ref responseMessage);

            }
            else if (MessageHandle<GetMessageIDMessageResponse>.Index != null
                     && header.MessageIndex == MessageHandle<GetMessageIDMessageResponse>.Index.Value)
            {
                var castMessage = (GetMessageIDMessageResponse) message;
                if (castMessage.Result)
                {


                    var type = Type.GetType(castMessage.TypeName);

                    var readId = MessageHandle.GetId(type);
                    if (!readId.HasValue)
                    {
                        MessageHandle.RegisterType(type, castMessage.TypeId);
                        handles.CreateHandle(type);
                    }
                    else if (readId.Value != castMessage.TypeId)
                    {
                        throw new Exception();
                    }


                }


            }
            return false;
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            var index = MessageHandle<T>.Index;

            if (!index.HasValue || !handles.ExistHandle(index.Value))
            {
                GetMessageIDMessageRequest request = new GetMessageIDMessageRequest(typeof(T));
                SendMessage<GetMessageIDMessageRequest>(ref request);
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
                GetMessageIDMessageRequest request = new GetMessageIDMessageRequest(typeof(T));
                SendMessage<GetMessageIDMessageRequest>(ref request);
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