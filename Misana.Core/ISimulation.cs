using System;
using System.Threading.Tasks;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;

namespace Misana.Core
{
    public interface ISimulation
    {
        Map CurrentMap { get;  }

        EntityManager Entities { get; }

        SimulationState State { get; }

        Task ChangeMap(Map map);

        void CreateEntity(string definitionName,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);
        void CreateEntity(EntityDefinition definition,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);
        void CreateEntity(int defintionId, int entityId,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);

        Task<int> CreatePlayer( PlayerInputComponent input, TransformComponent transform,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);
        Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform, int playerId,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);

        Task Start();
        void Update(GameTime gameTime);
    }
}