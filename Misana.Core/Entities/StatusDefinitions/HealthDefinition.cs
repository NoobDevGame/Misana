using System;
using System.IO;
using Misana.Core.Components.StatusComponents;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.StatusDefinitions
{
    public class HealthDefinition : ComponentDefinition<HealthComponent>
    {
        public float Current { get; set; }
        public float Max { get; set; }

        public HealthDefinition()
        {
            Current = Max = 100;
        }

        public HealthDefinition(float max)
        {
            Current = Max = max;
        }

        public HealthDefinition(float current, float max)
        {
            Current = current;
            Max = max;
        }

        public override void OnApplyDefinition(EntityBuilder entity, Map map, HealthComponent component, ISimulation sim)
        {
            component.Current = Current;
            component.Max = Max;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Current);
            bw.Write(Max);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Current = br.ReadSingle();
            Max = br.ReadSingle();
        }
    }
}