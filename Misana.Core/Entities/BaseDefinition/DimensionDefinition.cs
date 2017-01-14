using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Entities.BaseDefinition
{
    public class DimensionDefinition : ComponentDefinition<DimensionComponent>
    {
        public float Radius { get; set; }

        public DimensionDefinition()
        {
            Radius = 0.5f;
        }

        public DimensionDefinition(float radius)
        {
            Radius = radius;
        }

        public override void OnApplyDefinition(Entity entity, DimensionComponent component)
        {
            component.Radius = Radius;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Radius);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Radius = br.ReadSingle();
        }
    }
}