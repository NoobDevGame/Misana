using Misana.Network;

namespace Misana.Core.Effects.Messages
{
    [MessageDefinition]
    public struct OnDamageEffectMessage
    {
        public int EntityId;
        public float Damage;

        public OnDamageEffectMessage(int entityId, float damage)
        {
            EntityId = entityId;
            Damage = damage;
        }
    }
}