using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core
{
    public interface ISimulation
    {
        Map CurrentMap { get;  }

        EntityManager Entities { get; }

        void ChangeMap(Map map);

        int CreatePlayer(PlayerInputComponent input, TransformComponent transform);

        void Update(GameTime gameTime);
    }
}