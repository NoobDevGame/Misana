using System.Runtime.InteropServices;
using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class ChangeMapMessageRequest : NetworkRequest
    {
        public string Name;

        public ChangeMapMessageRequest(string name)
        {
            Name = name;
        }

        private ChangeMapMessageRequest(){}

        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }

    public class ChangeMapMessageResponse : NetworkResponse
    {
        public bool Result;

        private ChangeMapMessageResponse(){}
        public ChangeMapMessageResponse(bool result)
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
}