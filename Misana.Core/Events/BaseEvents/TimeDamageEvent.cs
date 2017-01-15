using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Components.StatusComponent;
using Misana.Core.Ecs;

namespace Misana.Core.Events.BaseEvents
{
    public class TimeDamageEvent : EventDefinition
    {
        public float DamagePerSeconds { get; set; }
        public TimeSpan EffectTime { get; set; }

        public TimeDamageEvent()
        {
            EffectTime = TimeSpan.FromSeconds(1);
            DamagePerSeconds = 10;
        }


        public override void Apply(Entity entity, World world)
        {
            var component = entity.Get<TimeDamageComponent>();
            var haveHealth = entity.Get<HealthComponent>() != null;

            if (component == null && haveHealth)
            {
                entity.Add<TimeDamageComponent>(p =>

                    {
                        p.EffectTime = EffectTime;
                        p.DamagePerSeconds = DamagePerSeconds;

                    }
                );
            }
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(DamagePerSeconds);
            bw.Write(EffectTime.TotalMilliseconds);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            DamagePerSeconds = br.ReadSingle();
            EffectTime = TimeSpan.FromMilliseconds(br.ReadDouble());
        }
    }
}