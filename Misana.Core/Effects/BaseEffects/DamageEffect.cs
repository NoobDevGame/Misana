using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Effects.BaseEffects
{
    public class DamageEffect : EffectDefinition
    {
        public float Damage { get; set; }

        public DamageEffect()
        {

        }

        public DamageEffect(float damage)
        {
            Damage = damage;
        }

        public override void Apply(Entity entity, ISimulation simulation)
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