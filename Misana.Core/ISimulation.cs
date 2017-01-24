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

        void CreateEntity(string definitionName);
        void CreateEntity(EntityDefinition defintion);
        void CreateEntity(int defintionId, int entityId);

        Task<int> CreatePlayer( PlayerInputComponent input, TransformComponent transform);
        Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform, int playerId);

        Task Start();
        void Update(GameTime gameTime);



    }
}