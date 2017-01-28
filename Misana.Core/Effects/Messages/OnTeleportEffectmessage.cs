using Misana.Network;

namespace Misana.Core.Effects.Messages
{
    [MessageDefinition]
    public struct OnTeleportEffectMessage
    {
        public int EntityId;
        public Vector2 Position;
        public int AreaId;

        public OnTeleportEffectMessage(int entityId, Vector2 position, int areaId)
        {
            EntityId = entityId;
            Position = position;
            AreaId = areaId;
        }
    }
}