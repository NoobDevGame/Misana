using Misana.Network;

namespace Misana.Core.Effects.Messages
{
    [MessageDefinition]
    public struct OnPickupEffectMessage
    {
        public int ParentEntityId;
        public int EntityId;

        public OnPickupEffectMessage(int parentEntityId, int entityId)
        {
            ParentEntityId = parentEntityId;
            EntityId = entityId;
        }
    }
}