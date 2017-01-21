using Misana.Core.Components;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition((ushort) MessageDataType.EntityPosition)]
    public struct EntityPositionMessage
    {
        public int entityId;

        public Vector2 position;

        public EntityPositionMessage(int entityId,TransformComponent component)
        {
            this.entityId = entityId;
            position = component.Position;
        }
    }
}