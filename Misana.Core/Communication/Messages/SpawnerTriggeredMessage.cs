using Misana.Core.Components;
using Misana.Core.Network;

namespace Misana.Core.Communication.Messages
{
    public class SpawnerTriggeredMessage : TcpGameMessage
    {
        public int SpawnerOwnerId;
        public int SpawnedEntityId;
        public Vector2 Position;
        public float Radius;
        public int AreaId;
        public bool Projectile;
        public Vector2 Move;
        private SpawnerTriggeredMessage(){}
        public SpawnerTriggeredMessage(int spawnerOwnerEntityId, int spawnedEntityId, TransformComponent spawnedTransform, ProjectileComponent spawnedProjectile)
        {
            SpawnerOwnerId = spawnerOwnerEntityId;
            SpawnedEntityId = spawnedEntityId;
            Position = spawnedTransform.Position;
            Radius = spawnedTransform.Radius;
            AreaId = spawnedTransform.CurrentAreaId;

            // ReSharper disable once AssignmentInConditionalExpression
            if (Projectile = spawnedProjectile != null)
            {
                Move = spawnedProjectile.Move;
            }
            else
            {
                Move = Vector2.Zero;
            }
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