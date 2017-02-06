using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class ExpiringComponentDefinition : ComponentDefinition<ExpiringComponent>
    {
        public TimeSpan TimeLeft;

        public ExpiringComponentDefinition(int ms)
        {
            TimeLeft = TimeSpan.FromMilliseconds(ms);
        }

        public ExpiringComponentDefinition(){}

        public override void OnApplyDefinition(EntityBuilder entity, Map map, ExpiringComponent component, ISimulation sim)
        {
            component.TimeLeft = TimeLeft;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            base.Serialize(version, bw);
            bw.Write(TimeLeft.TotalMilliseconds);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            base.Deserialize(version, br);
            TimeLeft = TimeSpan.FromMilliseconds(br.ReadDouble());
        }
    }
}