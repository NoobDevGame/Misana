using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;

namespace Misana.Core.Effects.BaseEffects
{
    public class ProjectileDamageEffect : EffectDefinition
    {

        public override void Apply(Entity entity, Entity self, ISimulation simulation)
        {
            var projectileComponent = self.Get<ProjectileComponent>();
            if (projectileComponent != null)
            {
                OnDamageEffectMessage message = new OnDamageEffectMessage(entity.Id,projectileComponent.BaseAttack);

                simulation.EffectMessenger.ApplyEffectSelf(ref message);
            }


        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
        }

        public override void Deserialize(Version version, BinaryReader br)
        {

        }
    }
}