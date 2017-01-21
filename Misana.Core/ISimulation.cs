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

        void ChangeMap(Map map);

        int CreateEntity(string definitionName);
        int CreateEntity(EntityDefinition defintion);
        int CreatePlayer( PlayerInputComponent input, TransformComponent transform);

        void Update(GameTime gameTime);
    }
}