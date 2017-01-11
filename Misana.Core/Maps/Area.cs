using System.Collections.Generic;
using System.Linq;

namespace Misana.Core.Maps
{
    public class Area
    {
        public Area(string name, int id, int width, int height, Vector2 spawnPoint, Layer[] layers)
        {
            Name = name;
            Id = id;
            Width = width;
            Height = height;
            SpawnPoint = spawnPoint;
            Layers = layers;

            MapTextures = new Dictionary<string, MapTexture>();

        }

        public string Name { get; }

        public int Id { get; }

        public int Width { get; }
        public int Height { get; }

        public Vector2 SpawnPoint { get; }

        public Dictionary<string, MapTexture> MapTextures { get; }

        public Dictionary<string, int> UsedTileSheets { get; }

        public Layer[] Layers { get; }

        public MapTexture GetMapTextures(int id)
        {
            return MapTextures.Values.First(m => m.Firstgid <= id && id <= m.Firstgid + m.Tilecount);
        }

        public bool IsCellBlocked(int x, int y)
        {
            if (x < 0 || y < 0)
                return true;

            if (x >= Width || y >= Height)
                return true;

            var index = x + Width * y;

            foreach (var layer in Layers)
            {
                var id = layer.Tiles[index];

                if (id == 0)
                    continue;

                var mapTexture = GetMapTextures(id);

                var property = mapTexture.GetTileProperty(id - mapTexture.Firstgid);

                if (property.Blocked)
                    return true;
            }

            return false;
        }

    }
}