using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class StartSimulationMessageRequest : NetworkRequest
    {
        public StartSimulationMessageRequest(){}
        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }


    public class StartSimulationMessageResponse : NetworkResponse
    {
        public bool Result;

        private StartSimulationMessageResponse(){}
        public StartSimulationMessageResponse(bool result)
        {
            Result = result;
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

    public class OnStartSimulationMessage : RpcMessage
    {
        public OnStartSimulationMessage(){}
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