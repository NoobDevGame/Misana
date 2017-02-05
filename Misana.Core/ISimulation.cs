using System.Threading.Tasks;
using Misana.Core.Ecs;
using Misana.Core.Maps;
using Misana.Core.Systems;

namespace Misana.Core
{
    public interface ISimulation
    {
        Map CurrentMap { get;  }

        EntityManager Entities { get; }

        SimulationState State { get; }

        SimulationMode Mode { get; }

        SpawnerSystem SpawnerSystem { get; }

        Task ChangeMap(Map map);

        Task Start();
        void Update(GameTime gameTime);

    }
}