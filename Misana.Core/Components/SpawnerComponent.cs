using System;
using System.Collections.Generic;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class SpawnerComponent : Component<SpawnerComponent>
    {
        public override void Reset()
        {
            AliveSpawnedEntityIds.Clear();
            TotalSpawned = 0;
            LastSpawned = 0;
            MaxAlive = 0;
            Template = null;
            CoolDown = 0;
            Active = false;
        }

        [Copy, Reset]
        public EntityBuilder Template;

        [Copy, Reset]
        public int MaxAlive;
        [Copy, Reset]
        public int TotalSpawnLimit;
        [Copy, Reset]
        public double CoolDown;
        [Copy, Reset]
        public Vector2 SpawnDirection;
        [Copy, Reset]
        public bool Projectile;

        [Copy, Reset]
        public bool Active;

        // Working Set
        [Reset]
        public int TotalSpawned;
        [Reset]
        public double LastSpawned;
        [Reset]
        public List<int> AliveSpawnedEntityIds = new List<int>();

        public override void CopyTo(SpawnerComponent other)
        {
            other.Template = Template.Copy();
            other.Active = Active;
            other.MaxAlive = MaxAlive;
            other.TotalSpawnLimit = TotalSpawnLimit;
            other.CoolDown = CoolDown;
            other.SpawnDirection = SpawnDirection;
        }
    }
}