using System;
using System.CodeDom;
using System.IO;
using System.Linq;

namespace Misana.Core.Maps.MapSerializers
{
    internal class MapSerializer_0_1 : MapSerializer
    {
        public override Version MapVersion  => new Version(0,1);

        protected override Map Deserialize(BinaryReader br)
        {
            var name = br.ReadString();
            var firstAreaId = br.ReadInt32();
            var firstAreaName = br.ReadString();

            var areaLength = br.ReadInt32();
            Area[] areas = new Area[areaLength];
            for (int i = 0; i < areaLength; i++)
            {
                areas[i] = DeserializeArea(br);
            }

            return  new Map(name,areas[firstAreaId -1],areas);
        }

        private Area DeserializeArea(BinaryReader br)
        {
            var id = br.ReadInt32();
            var name = br.ReadString();
            var width = br.ReadInt32();
            var height = br.ReadInt32();

            Vector2 spawnpoint = new Vector2(br.ReadSingle(),br.ReadSingle());

            var layerlenght = br.ReadInt32();
            Layer[] layers = new Layer[layerlenght];

            for (int i = 0; i < layerlenght; i++)
            {
                layers[i] = DeserializeLayer(br);
            }

            Area area = new Area(name,id,width,height,spawnpoint,layers);

            var texturecount = br.ReadInt32();
            for (int i = 0; i < texturecount; i++)
            {
                MapTexture texture = DeserializeTexture(br);
                //area.MapTextures.Add(texture.Key,texture);
            }

            return area;

        }

        private Layer DeserializeLayer(BinaryReader br)
        {
            var id = br.ReadInt32();

            var lenght = br.ReadInt32();
            Tile[] tiles = new Tile[lenght];
            //for (int i = 0; i < lenght; i++)
            //{
            //    tiles[i] = br.ReadInt32();
            //}

            return new Layer(id,tiles);
        }

        private MapTexture DeserializeTexture(BinaryReader br)
        {
            var key = br.ReadString();
            var firstid = br.ReadInt32();
            var tilecount = br.ReadInt32();
            var spacing = br.ReadInt32();
            var tileheight = br.ReadInt32();
            var tilewidth = br.ReadInt32();
            var columns = br.ReadInt32();

            var texture = new MapTexture(key,firstid,tilecount,spacing,tileheight,tilewidth,columns);

            for (int i = 0; i < tilecount; i++)
            {
                TileProperty tile = new TileProperty();
                tile.Blocked = br.ReadBoolean();
                texture.SetTileProperty(i,tile);
            }

            return texture;
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
            //bw.Write((area.MapTextures.Count));
            //foreach (var texture in area.MapTextures.Select(i => i.Value))
            //{
            //    bw.Write(texture.Key);
            //    bw.Write(texture.Firstgid);
            //    bw.Write(texture.Tilecount);
            //    bw.Write(texture.Spacing);
            //    bw.Write(texture.Tileheight);
            //    bw.Write(texture.Tilewidth);
            //    bw.Write(texture.Columns);

            //    for (int i = 0; i < texture.Tilecount; i++)
            //    {
            //        var property = texture.GetTileProperty(i);
            //        bw.Write(property.Blocked);
            //    }
            //}
        }

        private void SerializeLayer(Layer layer, BinaryWriter bw)
        {
            bw.Write(layer.Id);
            
            bw.Write(layer.Tiles.Length);
            //for (int i = 0; i < layer.Tiles.Length; i++)
                //bw.Write(layer.Tiles[i]);
        }
    }
}