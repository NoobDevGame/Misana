using Misana.Core.Ecs.Meta;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition]
    public struct EntityDeathMessage
    {
        [Copy, Reset]
        public int EntityId;

        public EntityDeathMessage(int entityId)
        {
            EntityId = entityId;
        }
    }
}