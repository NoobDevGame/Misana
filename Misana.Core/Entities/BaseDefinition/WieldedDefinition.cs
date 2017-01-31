using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class WieldedDefinition : ComponentDefinition<WieldedComponent>
    {
        public Vector2 Offset { get; set; }

        public WieldedDefinition()
        {

        }

        public WieldedDefinition(float x, float y)
        {
            Offset = new Vector2(x,y);
        }

        public WieldedDefinition(Vector2 offset)
        {
            Offset = offset;
        }

        public override void OnApplyDefinition(EntityBuilder entity, Map map, WieldedComponent component, ISimulation sim)
        {
            component.Offset = Offset;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Offset.X);
            bw.Write(Offset.Y);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Offset = new Vector2(br.ReadSingle(),br.ReadSingle());
        }
    }
}