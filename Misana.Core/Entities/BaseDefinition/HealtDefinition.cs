using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class HealtDefinition : ComponentDefinition<HealthComponent>
    {
        public float Current { get; set; }
        public float Max { get; set; }

        public HealtDefinition()
        {
            Current = Max = 100;
        }

        public HealtDefinition(float max)
        {
            Current = Max = max;
        }

        public HealtDefinition(float current, float max)
        {
            Current = current;
            Max = max;
        }

        public override void OnApplyDefinition(Entity entity, Map map, HealthComponent component)
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