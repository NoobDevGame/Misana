using System;
using System.IO;
using Misana.Core.Components.Events;
using Misana.Core.Ecs;
using Misana.Core.Events;

namespace Misana.Core.Effects.Conditions
{
    public class FlagCondition : EffectCondition
    {
        public bool Not { get; set; }
        public string Name { get; set; }

        public FlagCondition()
        {

        }

        public FlagCondition(string name)
        {
            Name = name;
        }

        public FlagCondition(string name,bool not)
        {
            Name = name;
            Not = not;
        }

        public override bool Test(Entity entity, ISimulation simulation)
        {
            var flagComponent = entity.Get<EntityFlagComponent>();

            if (flagComponent == null)
                return true;

            var value = flagComponent.Get(Name);

            if (Not)
                return !value;

            return value;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Name);
            bw.Write(Not);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Name = br.ReadString();
            Not = br.ReadBoolean();
        }

    }
}