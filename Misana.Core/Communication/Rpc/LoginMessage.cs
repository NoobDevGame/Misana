using System.Runtime.InteropServices;
using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class LoginMessageRequest : NetworkRequest
    {
        public string Name;

        public LoginMessageRequest(string name)
        {
            Name = name;
        }

        private LoginMessageRequest(){}
        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }

    public class LoginMessageResponse : NetworkResponse
    {
        public int Id;

        public LoginMessageResponse(int id)
        {
            Id = id;
        }
        private LoginMessageResponse(){}
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