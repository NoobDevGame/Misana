using System.Collections.Generic;
using Misana.Core.Client;
using Misana.Core.Ecs;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class HotJoinedMessage : RpcMessage
    {
        public List<Entity> Entities;
        public string Name;
        public int PlayerId;

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