using Misana.Core.Network;

namespace Misana.Core.Communication.Messages
{
    public class EntityHealthMessage : UdpGameMessage
    {
        public int EntityId;
        public float Health;

        public EntityHealthMessage(int entityId, float health)
        {
            EntityId = entityId;
            Health = health;
        }

        private EntityHealthMessage(){}
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