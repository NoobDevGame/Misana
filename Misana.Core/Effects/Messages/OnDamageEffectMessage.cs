using Misana.Core.Network;

namespace Misana.Core.Effects.Messages
{
    public class OnDamageEffectMessage : TcpGameMessage
    {
        public int EntityId;
        public float Damage;

        public OnDamageEffectMessage(int entityId, float damage)
        {
            EntityId = entityId;
            Damage = damage;
        }

        private OnDamageEffectMessage(){}

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