using Misana.Core.Network;

namespace Misana.Core.Effects.Messages
{
    public class OnPickupEffectMessage : TcpGameMessage
    {
        public int ParentEntityId;
        public int EntityId;
        private OnPickupEffectMessage(){}
        public OnPickupEffectMessage(int parentEntityId, int entityId)
        {
            ParentEntityId = parentEntityId;
            EntityId = entityId;
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