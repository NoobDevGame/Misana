using Misana.Network;

namespace Misana.Core.Effects.Messages
{
    [MessageDefinition]
    public struct OnCreateEntityEffectMessage
    {
        public int EntityId;

        public OnCreateEntityEffectMessage(int entityId)
        {
            EntityId = entityId;
        }
    }
}