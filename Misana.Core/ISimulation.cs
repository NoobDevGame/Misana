using System;
using System.Threading.Tasks;
using Misana.Core.Communication;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Events;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core
{
    public interface ISimulation
    {
        Map CurrentMap { get;  }

        EntityManager Entities { get; }

        SimulationState State { get; }

        SimulationMode Mode { get; }

        EffectApplicator EffectMessenger { get; }

        Task ChangeMap(Map map);

        Task<int> CreateEntity(string definitionName,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);
        Task<int> CreateEntity(string definitionName, int entityId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback);
        Task<int> CreateEntity(EntityDefinition definition,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);
        Task<int> CreateEntity(int defintionId,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);
        Task<int> CreateEntity(int defintionId, int entityId,Action<EntityBuilder> createCallback,Action<Entity> createdCallback);

        Task Start();
        void Update(GameTime gameTime);

    }
}