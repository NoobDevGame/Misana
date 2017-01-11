using System;
using System.IO;
using System.Linq;

namespace Misana.Core.Map.MapSerializers
{
    class MapSerializer_0_1 : MapSerializer
    {
        public override Version MapVersion  => new Version(0,1);

        protected override Map Deserialize(BinaryReader br)
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(Map map,BinaryWriter bw)
        {
            bw.Write(map.Name);
            bw.Write(map.StartArea.Id);
            bw.Write(map.StartArea.Name);

            //Areas
            bw.Write(map.Areas.Length);
            foreach (var area in map.Areas)
            {
                SerializeArea(area, bw);
            }
        }

        private void SerializeArea(Area area, BinaryWriter bw)
        {
            //Info
            bw.Write(area.Id);
            bw.Write(area.Name);
            bw.Write(area.Width);
            bw.Write(area.Height);

            //SpwanPoint
            bw.Write(area.SpawnPoint.X);
            bw.Write(area.SpawnPoint.Y);

            //Layers
            bw.Write(area.Layers.Length);
            foreach (var layer in area.Layers)
            {
                SerializeLayer(layer, bw);
            }


            //Textures
            bw.Write((area.MapTextures.Count));
            foreach (var texture in area.MapTextures.Select(i => i.Value))
            {
                bw.Write(texture.Key);
                bw.Write(texture.Firstgid);
                bw.Write(texture.Tilecount);
                bw.Write(texture.Spacing);
                bw.Write(texture.Tileheight);
                bw.Write(texture.Tilewidth);

                bw.Write(texture.Tilecount);
                for (int i = 0; i < texture.Tilecount; i++)
                {
                    var property = texture.GetTileProperty(i);
                    bw.Write(property.Blocked);
                }
            }
        }

        private void SerializeLayer(Layer layer, BinaryWriter bw)
        {
            bw.Write(layer.Id);
            
            bw.Write(layer.Tiles.Length);
            for (int i = 0; i < layer.Tiles.Length; i++)
                bw.Write(layer.Tiles[i]);
        }
    }
}