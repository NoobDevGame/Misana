using System.Collections.Generic;
using System.Linq;

namespace Misana.Core.Maps
{
    public class Area
    {
        public string Name { get; }

        public int Id { get; }

        public int Width { get; }
        public int Height { get; }

        public string TilesheetName(int id) => Tilesheets[id];

        public Vector2 SpawnPoint { get; set; }

        public Dictionary<int,string> Tilesheets { get; set; }

        public List<Layer> Layers { get; set; }

        public Area(string name, int id, int width, int height, Vector2 spawnPoint, List<Layer> layers)
        {
            Name = name;
            Id = id;
            Width = width;
            Height = height;
            SpawnPoint = spawnPoint;
            Layers= layers;

            Tilesheets = new Dictionary<int, string>();
        }

        public Area(string name, int id, int width, int height)
        {
            Name = name;
            Id = id;
            Width = width;
            Height = height;

            Tilesheets = new Dictionary<int, string>();
        }

        public bool IsCellBlocked(int x, int y)
        {
            if (x < 0 || y < 0)
                return true;

            if (x >= Width || y >= Height)
                return true;

            var index = GetTileIndex(x, y);

            foreach (var layer in Layers)
            {
                if (layer.Tiles[index].Blocked)
                    return true;
            }

            return false;
        }

        public int GetTileIndex(int x, int y)
        {
            return x + Width * y;
        }

        public int AddTilesheet(string tilesheet)
        {
            var index = Tilesheets.Count + 1;

            if (Tilesheets.ContainsValue(tilesheet))
                return Tilesheets.Where(t => t.Value == tilesheet).First().Key;

            Tilesheets.Add(index, tilesheet);

            return index;

        }

    }
}