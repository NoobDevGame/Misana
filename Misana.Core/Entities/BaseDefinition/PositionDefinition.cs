using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class PositionDefinition : ComponentDefinition<PositionComponent>
    {
        public Vector2 Position { get; set; }
        public int AreaId { get; set; }

        public PositionDefinition()
        {

        }

        public PositionDefinition(Vector2 position,int areaId)
        {
            Position = position;
            AreaId = areaId;
        }

        public PositionDefinition(Vector2 position,Area area)
        {
            Position = position;
            AreaId = area.Id;
        }

        public PositionDefinition(Area area)
        {
            Position = area.SpawnPoint;
            AreaId = area.Id;
        }

        public override void OnApplyDefinition(Entity entity, Map map, PositionComponent component)
        {
            component.CurrentArea = map.GetAreaById(AreaId);
            component.Position = Position;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(AreaId);
            bw.Write(Position.X);
            bw.Write(Position.Y);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            AreaId = br.ReadInt32();
            Position = new Vector2(br.ReadSingle(),br.ReadSingle());
        }
    }
}