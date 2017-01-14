using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class CharacterRenderDefinition : ComponentDefinition<CharacterRenderComponent>
    {
        public Index2 TilePosition { get; set; }

        public CharacterRenderDefinition()
        {
            TilePosition = new Index2(0,0);
        }

        public CharacterRenderDefinition(Index2 tilePosition)
        {
            TilePosition = tilePosition;
        }

        public override void OnApplyDefinition(Entity entity, Map map, CharacterRenderComponent component)
        {
            component.TilePosition = TilePosition;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(TilePosition.X);
            bw.Write(TilePosition.Y);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            var x = br.ReadInt32();
            var y = br.ReadInt32();

            TilePosition = new Index2(x,y);
        }
    }
}