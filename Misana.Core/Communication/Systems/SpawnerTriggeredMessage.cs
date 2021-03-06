﻿using Misana.Core.Components;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    [MessageDefinition]
    public struct SpawnerTriggeredMessage
    {
        public int SpawnerOwnerId;
        public int SpawnedEntityId;
        public Vector2 Position;
        public float Radius;
        public int AreaId;
        public bool Projectile;
        public Vector2 Move;

        public SpawnerTriggeredMessage(int spawnerOwnerEntityId, int spawnedEntityId, TransformComponent spawnedTransform, ProjectileComponent spawnedProjectile)
        {
            SpawnerOwnerId = spawnerOwnerEntityId;
            SpawnedEntityId = spawnedEntityId;
            Position = spawnedTransform.Position;
            Radius = spawnedTransform.Radius;
            AreaId = spawnedTransform.CurrentArea.Id;

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
    }
}