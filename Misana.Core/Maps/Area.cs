using System.Collections.Generic;

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

        public Layer[] Layers { get; }

        public MapTexture GetMapTextures(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool IsCellBlocked(int x, int y)
        {
            throw new System.NotImplementedException();
        }

    }
}