using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Effects.BaseEffects
{
    public class RemoveSelfEffect : EffectDefinition
    {
        public override void Apply(Entity entity, ISimulation simulation)
        {
            simulation.Entities.RemoveEntity(entity);
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
        }
    }
}