using System.Collections.Generic;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Communication
{
    public class DummyServer : ISimulation
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        public DummyServer()
        {
            BaseSimulation = new Simulation(null,null);
        }

        public void ChangeMap(Map map)
        {
            BaseSimulation.ChangeMap(map);
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            return BaseSimulation.CreatePlayer(input, transform);
        }

        public void Update(GameTime gameTime)
        {
            BaseSimulation.Update(gameTime);
        }
    }
}