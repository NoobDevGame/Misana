using System;
using System.Collections.Generic;
using Misana.Core.Ecs;

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

        // Fixed
        public EntityBuilder Template;
        public int MaxAlive;
        public int TotalSpawnLimit;
        public double CoolDown;

        // Config
        public bool Active;

        // Working Set
        public int TotalSpawned;
        public double LastSpawned;
        public List<int> AliveSpawnedEntityIds = new List<int>();
        
        
        public override void CopyTo(SpawnerComponent other)
        {
            other.Template = Template.Copy();
            other.Active = Active;
            other.MaxAlive = MaxAlive;
            other.TotalSpawnLimit = TotalSpawnLimit;
            other.CoolDown = CoolDown;
        }
    }
}