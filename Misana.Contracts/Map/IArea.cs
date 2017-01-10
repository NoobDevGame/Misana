using System.Collections.Generic;
using Misana.Contracts.Entity;

namespace Misana.Contracts.Map
{
    public interface IArea
    {
        string Name { get; }

        int Id { get; }

        int Width { get; }
        int Height { get; }
        Vector2 SpawnPoint { get; }

        Dictionary<string, MapTexture> MapTextures { get; } //TODO enumerable

        ILayer[] Layers { get; }
        IEnumerable<IEntity> Entities { get; }

        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);

        MapTexture GetMapTextures(int id);

        bool IsCellBlocked(int x, int y);
        bool ContainsEntity(IEntity entity);
    }
}
