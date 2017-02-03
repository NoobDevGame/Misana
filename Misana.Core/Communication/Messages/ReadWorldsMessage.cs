using System.Runtime.InteropServices;
using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class ReadWorldsMessageRequest : NetworkRequest
    {
        private ReadWorldsMessageRequest(){}
        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }

    public class WorldInformationMessage : NetworkResponse
    {
        public int WorldId;

        public string Name;

        private WorldInformationMessage(){}
        public WorldInformationMessage(int worldId, string name)
        {
            WorldId = worldId;
            Name = name;
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