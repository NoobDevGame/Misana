using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Misana.Core.Entities;

namespace Misana.Core.Maps.MapSerializers
{
    internal class MapSerializer_0_2 : MapSerializer
    {
        public override Version MapVersion => new Version(0,2);

        protected override Map Deserialize(BinaryReader br)
        {
            var mapName = br.ReadString();
            var startAreaId = br.ReadInt32();



            //GlobalEntityDefintions
            var definitionLength = br.ReadInt32();
            Dictionary<string,EntityDefinition> globalEntityDefinitions = new Dictionary<string, EntityDefinition>();

            for (int i = 0; i < definitionLength; i++)
            {
                var definitionName = br.ReadString();
                var definition = DeserializeEntityDefinition(br);
                globalEntityDefinitions.Add(definitionName,definition);
            }

            //Areas
            var areaLength = br.ReadInt32();
            var areas = new Area[areaLength];
            for (int i = 0; i < areaLength; i++)
            {
                areas[i] = DeserializeArea(br);
            }

            var startIndex = areas.ToList().FindIndex(t => t.Id == startAreaId);

            var map = new Map(mapName, areas[startIndex], areas.ToList());

            map.GlobalEntityDefinitions = globalEntityDefinitions;

            return map;
        }

        private Area DeserializeArea(BinaryReader br)
        {
            var id = br.ReadInt32();
            var name = br.ReadString();

            var height = br.ReadInt32();
            var width = br.ReadInt32();

            var spawnX = br.ReadSingle();
            var spawnY = br.ReadSingle();

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

            var entityCount = br.ReadInt32();
            for (int i = 0; i < entityCount; i++)
            {
                AreaEntity entity = new AreaEntity();
                entity.Name = br.ReadString();
                entity.Definition = DeserializeEntityDefinition(br);
                a.Entities.Add(entity);
            }


            a.Tilesheets = tilesheets;;
            return a;
        }

        private EntityDefinition DeserializeEntityDefinition(BinaryReader br)
        {
            EntityDefinition definition = new EntityDefinition();

            definition.Name =  br.ReadString();

            var definitionCount = br.ReadInt32();

            for (int i = 0; i < definitionCount; i++)
            {
                var componentType = br.ReadString();
                var componentDefinition = (ComponentDefinition)Activator.CreateInstance(Type.GetType( componentType));
                componentDefinition.Deserialize(MapVersion,br);
            }

            return definition;
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

            if (sId == 2)
            {

            }

            var blocked = br.ReadBoolean();
            return new Tile(tId, sId, blocked);
        }

        protected override void Serialize(Map map, BinaryWriter bw)
        {
            bw.Write(map.Name);
            bw.Write(map.StartArea.Id);

            //GlobalEntityDefintions
            bw.Write(map.GlobalEntityDefinitions.Count);
            foreach (var definition in map.GlobalEntityDefinitions)
            {
                bw.Write(definition.Key);
                SerializeEntityDefinition(definition.Value,bw);
            }

            //Areas
            bw.Write(map.Areas.Count);
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
            bw.Write(area.Layers.Count);
            foreach (var layer in area.Layers)
            {
                SerializeLayer(layer, bw);
            }

            //Tilesets
            bw.Write(area.Tilesheets.Count);
            foreach (var tilesheet in area.Tilesheets)
            {
                bw.Write(tilesheet.Key);
                bw.Write(tilesheet.Value);
            }

            //Entities
            bw.Write(area.Entities.Count);
            foreach (var entity in area.Entities)
            {
                bw.Write(entity.Name);
                SerializeEntityDefinition(entity.Definition,bw);
            }
        }

        private void SerializeEntityDefinition(EntityDefinition entityDefinition, BinaryWriter bw)
        {
            bw.Write(entityDefinition.Name);
            bw.Write(entityDefinition.Definitions.Count);
            foreach (var definition in entityDefinition.Definitions)
            {
                bw.Write(definition.GetType().AssemblyQualifiedName);
                definition.Serialize(MapVersion,bw);
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