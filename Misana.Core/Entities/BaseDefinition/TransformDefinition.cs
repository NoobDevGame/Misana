using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class TransformDefinition : ComponentDefinition<TransformComponent>
    {
        public Vector2 Position { get; set; }
        public int AreaId { get; set; }
        public float Radius { get; set; }

        public TransformDefinition()
        {
            Radius = 0.5f;
        }

        public TransformDefinition(Vector2 position,int areaId, float radius)
        {
            Position = position;
            AreaId = areaId;
            Radius = radius;
        }

        public TransformDefinition(Vector2 position,Area area, float radius)
        {
            Position = position;
            AreaId = area.Id;
            Radius = radius;
        }

        public TransformDefinition(Area area, float radius)
        {
            Position = area.SpawnPoint;
            AreaId = area.Id;
            Radius = radius;
        }

        public override void OnApplyDefinition(Entity entity, Map map, TransformComponent component)
        {
            component.CurrentArea = map.GetAreaById(AreaId);
            component.Position = Position;
            component.Radius = Radius;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(AreaId);
            bw.Write(Position.X);
            bw.Write(Position.Y);
            bw.Write(Radius);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            AreaId = br.ReadInt32();
            Position = new Vector2(br.ReadSingle(),br.ReadSingle());
            Radius = br.ReadSingle();
        }
    }
}