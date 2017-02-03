using System.Runtime.InteropServices;
using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class CreateWorldMessageRequest : NetworkRequest
    {
        public string Name;

        public CreateWorldMessageRequest(string name)
        {
            Name = name;
        }
        private CreateWorldMessageRequest(){}

        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }

    public class CreateWorldMessageResponse : NetworkResponse
    {
        public bool Result;
        public int Id;
        public int FirstLocalId;

        private CreateWorldMessageResponse(){}
        public CreateWorldMessageResponse(bool result, int id, int firstLocalId)
        {
            Result = result;
            Id = id;
            FirstLocalId = firstLocalId;
        }

        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }
}