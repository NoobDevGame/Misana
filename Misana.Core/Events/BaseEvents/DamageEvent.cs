﻿using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Events.BaseEvents
{
    public class DamageEvent : EventDefinition
    {
        public float Damage { get; set; }

        public DamageEvent()
        {

        }

        public DamageEvent(float damage)
        {
            Damage = damage;
        }

        public override void Apply(Entity entity, World world)
        {
            var healthComponet = entity.Get<HealthComponent>();

            if (healthComponet != null)
                healthComponet.Current -= Damage;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Damage);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Damage = br.ReadSingle();
        }
    }
}