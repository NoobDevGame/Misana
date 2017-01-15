using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Events.BaseEvents
{
    public class TeleportEvent : EventDefinition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int AreaId { get; set; }

        public bool CenterOfBlock { get; set; } = true;

        public TeleportEvent()
        {

        }

        public TeleportEvent(int x, int y , int areaID)
        {
            X = x;
            Y = y;
            AreaId = areaID;
        }

        public override void Apply(Entity entity, World world)
        {
            var positionComponent = entity.Get<PositionComponent>();

            if (positionComponent != null)
            {
                var area = world.CurrentMap.GetAreaById(AreaId);
                positionComponent.CurrentArea = area;
                positionComponent.Position = new Vector2(X,Y);
                if (CenterOfBlock)
                {
                    positionComponent.Position += new Vector2(0.5f,0.5f);
                }
            }
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(AreaId);
            bw.Write(CenterOfBlock);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            X = br.ReadInt32();
            Y = br.ReadInt32();
            AreaId = br.ReadInt32();
            CenterOfBlock = br.ReadBoolean();
        }
    }
}