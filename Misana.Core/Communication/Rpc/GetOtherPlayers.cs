using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class GetOtherPlayersMessageRequest : NetworkRequest
    {
        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }


    public class PlayerInfoMessage : NetworkResponse
    {
        public int PlayerId;

        public string Name;


        public PlayerInfoMessage(int playerId, string name)
        {
            PlayerId = playerId;
            Name = name;
        }

        private PlayerInfoMessage(){}

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