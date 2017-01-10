using System.Collections.Generic;
using Misana.Contracts;
using Misana.Contracts.Entity;
using Misana.Contracts.Map;

namespace Misana.Core.Map
{
    public class Area : IArea
    {
        public Area(string name, int id, int width, int height, Vector2 spawnPoint, ILayer[] layers)
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

        public ILayer[] Layers { get; }

        private List<IEntity> entities = new List<IEntity>();
        public IEnumerable<IEntity> Entities => entities;

        public void AddEntity(IEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEntity(IEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public MapTexture GetMapTextures(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool IsCellBlocked(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsEntity(IEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}