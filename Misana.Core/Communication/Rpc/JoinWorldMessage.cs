using System.Runtime.InteropServices;
using Misana.Core.Client;
using Misana.Core.Network;
using Misana.Core.Server;

namespace Misana.Core.Communication.Messages
{
    public class JoinWorldMessageRequest : NetworkRequest
    {
        public int Id;

        public JoinWorldMessageRequest(int id)
        {
            Id = id;
        }

        private JoinWorldMessageRequest(){}
        public override void HandleOnClient(IClientRpcMessageHandler h)
        {
            h.Handle(this);
        }

        public override void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client)
        {
            h.Handle(this, messageId, client);
        }
    }

    public class JoinWorldMessageResponse : NetworkResponse
    {
        public bool Result;
        public bool HaveWorld;

        public int FirstLocalEntityId;

        public string MapName;

        private JoinWorldMessageResponse(){}
        public JoinWorldMessageResponse(bool result, bool haveWorld, string mapName, int firstLocalEntityId)
        {
            Result = result;
            HaveWorld = haveWorld;
            MapName = mapName;
            FirstLocalEntityId = firstLocalEntityId;
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

    public class OnJoinWorldMessage : RpcMessage
    {
        public int PlayerId;
        public string Name;

        private OnJoinWorldMessage(){}
        public OnJoinWorldMessage(int playerId, string name)
        {
            PlayerId = playerId;
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