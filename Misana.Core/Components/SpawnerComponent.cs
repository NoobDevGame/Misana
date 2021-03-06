﻿using System;
using System.Collections.Generic;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class SpawnerComponent : Component<SpawnerComponent>
    {
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
    }
}