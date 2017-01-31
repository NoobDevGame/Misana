using System;
using System.IO;
using Misana.Core.Components.Events;
using Misana.Core.Ecs;
using Misana.Core.Events;

namespace Misana.Core.Effects.BaseEffects
{
    public class SetEntityFlagEffect : EffectDefinition
    {

        public string Name { get; set; } = "Unnamed";

        public SetEntityFlagEffect()
        {

        }

        public SetEntityFlagEffect(string name)
        {
            Name = name;
        }

        public override void Apply(Entity entity, Entity self, ISimulation simulation)
        {
            var flagComponent = entity.Get<EntityFlagComponent>();

            flagComponent?.Set(Name);
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Name);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Name = br.ReadString();
        }
    }
}