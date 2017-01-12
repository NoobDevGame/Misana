using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Misana.Core.Maps.MapSerializers
{
    internal class MapSerializer_0_1 : MapSerializer
    {
        public override Version MapVersion => new Version(0,1);

        protected override Map Deserialize(BinaryReader br)
        {
            var name = br.ReadString();
            var startAreaId = br.ReadInt32();

            var length = br.ReadInt32();

            var areas = new Area[length];

            for (int i = 0; i < length; i++)
            {
                areas[i] = DeserializeArea(br);
            }

            var startIndex = areas.ToList().FindIndex(t => t.Id == startAreaId);

            return new Map(name, areas[startIndex], areas);
        }

        private Area DeserializeArea(BinaryReader br)
        {
            var id = br.ReadInt32();
            var name = br.ReadString();

            var height = br.ReadInt32();
            var width = br.ReadInt32();

            var spawnX = br.ReadInt32();
            var spawnY = br.ReadInt32();

            var layerLength = br.ReadInt32();

            var layers = new Layer[layerLength];

            for (int i = 0; i < layerLength; i++)
            {
                var l = DeserializeLayer(br);
                layers[i] = l;
            }

            var tilesheetLength = br.ReadInt32();

            var tilesheets = new Dictionary<int, string>();

            for (int b = 0; b < tilesheetLength; b++)
            {
                var tId = br.ReadInt32();
                var tName = br.ReadString();

                tilesheets.Add(tId, tName);
            }

            Area a = new Area(name, id, width, height, new Vector2(spawnX, spawnY), layers.ToList<Layer>());
            a.Tilesheets = tilesheets;
            return a;
        }

        private Layer DeserializeLayer(BinaryReader br)
        {
            var id = br.ReadInt32();

            var length = br.ReadInt32();

            var tiles = new Tile[length];

            for (int i = 0; i < length; i++)
            {
                var t = DeserializeTile(br);
                tiles[i] = t;
            }

            return new Layer(id, tiles);
        }

        private Tile DeserializeTile(BinaryReader br)
        {
            var tId = br.ReadInt32();
            var sId = br.ReadInt32();
            var blocked = br.ReadBoolean();
            return new Tile(tId, sId, blocked);
        }

        protected override void Serialize(Map map, BinaryWriter bw)
        {
            bw.Write(map.Name);
            bw.Write(map.StartArea.Id);

            bw.Write(map.Areas.Length);
            foreach (var area in map.Areas)
            {
                SerializeArea(area, bw);
            }
        }

        private void SerializeArea(Area area, BinaryWriter bw)
        {
            //General Information
            bw.Write(area.Id);
            bw.Write(area.Name);

            //Dimensions
            bw.Write(area.Width);
            bw.Write(area.Height);

            //SpawnPoint
            bw.Write(area.SpawnPoint.X);
            bw.Write(area.SpawnPoint.Y);

            //Layers
            bw.Write(area.Layers.Length);
            foreach (var layer in area.Layers)
            {
                SerializeLayer(layer, bw);
            }

            bw.Write(area.Tilesheets.Count);
            foreach (var tilesheet in area.Tilesheets)
            {
                bw.Write(tilesheet.Key);
                bw.Write(tilesheet.Value);
            }
        }

        private void SerializeLayer(Layer layer, BinaryWriter bw)
        {
            bw.Write(layer.Id);

            bw.Write(layer.Tiles.Length);
            foreach (var tile in layer.Tiles)
            {
                SerializeTile(tile, bw);
            }
        }

        private void SerializeTile(Tile tile, BinaryWriter bw)
        {
            bw.Write(tile.TextureID);
            bw.Write(tile.TilesheetID);
            bw.Write(tile.Blocked);
        }
    }
}