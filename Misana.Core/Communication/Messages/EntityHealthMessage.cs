using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition(UseUDP = true)]
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