using System;
using System.Drawing;
using System.IO;
using Misana.Core.Components.Events;
using Misana.Core.Ecs;

namespace Misana.Core.Events.BaseEvents
{
    public class SetEntityFlagEvent : EventDefinition
    {

        public string Name { get; set; } = "Unnamed";

        public SetEntityFlagEvent()
        {

        }

        public SetEntityFlagEvent(string name)
        {
            Name = name;
        }

        public override void Apply(Entity entity, World world)
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