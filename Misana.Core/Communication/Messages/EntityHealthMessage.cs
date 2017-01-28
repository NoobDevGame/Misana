using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition]
    public struct EntityHealthMessage
    {
        public int EntityId;
        public float Health;

        public EntityHealthMessage(int entityId, float health)
        {
            EntityId = entityId;
            Health = health;
        }
    }
}