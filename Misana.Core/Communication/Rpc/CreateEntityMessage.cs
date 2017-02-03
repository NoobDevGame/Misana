using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class CreateEntityMessageRequest : NetworkRequest
    {
        public int DefinitionId;

        public CreateEntityMessageRequest(int definitionId)
        {
            DefinitionId = definitionId;
        }

        private CreateEntityMessageRequest(){}

        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }

    public class CreateEntityMessageResponse : NetworkResponse
    {
        public bool Result;
        public int EntityId;

        private CreateEntityMessageResponse(){}
        public CreateEntityMessageResponse(bool result,int entityId)
        {
            Result = result;
            EntityId = entityId;
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

    public class OnCreateEntityMessage : TcpGameMessage
    {
        public int EntityId;
        public int DefinitionId;

        private OnCreateEntityMessage(){}
        public OnCreateEntityMessage(int entityId, int definitionId)
        {
            EntityId = entityId;
            DefinitionId = definitionId;
        }

        public override void ApplyOnClient(IClientGameMessageApplicator a)
        {
            a.Apply(this);
        }

        public override void ApplyOnServer(IServerGameMessageApplicator a, IClientOnServer client)
        {
            a.Apply(this, client);
        }
    }
}