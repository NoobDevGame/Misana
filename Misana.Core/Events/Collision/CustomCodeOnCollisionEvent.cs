﻿using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Collision
{
    public class CustomCodeOnCollisionEvent : OnCollisionEvent
    {
        private readonly Action<EntityManager, Entity, World> _action;

        public CustomCodeOnCollisionEvent(Action<EntityManager, Entity, World> action)
        {
            _action = action;
        }
        

        public override void Serialize(Version version, BinaryWriter bw)
        {
            
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            
        }

        protected override bool ApplyToEntity(EntityManager manager, Entity target, World world)
        {
            _action(manager, target, world);
            return true;
        }
    }
}