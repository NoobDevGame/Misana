using Misana.Core.Network;

namespace Misana.Core.Effects.Messages
{
    public class OnTeleportEffectMessage : TcpGameMessage
    {
        public int EntityId;
        public Vector2 Position;
        public int AreaId;

        private OnTeleportEffectMessage(){}
        public OnTeleportEffectMessage(int entityId, Vector2 position, int areaId)
        {
            EntityId = entityId;
            Position = position;
            AreaId = areaId;
        }

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