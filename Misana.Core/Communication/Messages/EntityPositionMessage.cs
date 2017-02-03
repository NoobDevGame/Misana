using Misana.Core.Components;
using Misana.Core.Network;

namespace Misana.Core.Communication.Messages
{
    public class EntityPositionMessage : UdpGameMessage
    {
        public int entityId;

        public Vector2 position;
        public Vector2 Facing;

        public EntityPositionMessage(int entityId,TransformComponent component)
        {
            this.entityId = entityId;
            position = component.Position;
            Facing = Vector2.Zero;
        }

        private EntityPositionMessage(){}

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