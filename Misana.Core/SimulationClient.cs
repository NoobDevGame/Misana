using System.Collections.Generic;
using Misana.Core.Communication;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core
{
    public class SimulationClient : ISimulation
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        private ISimulation _serverClient;

        public SimulationClient(List<BaseSystem> beforSystems,List<BaseSystem> afterSystems)
        {
            BaseSimulation = new Simulation(beforSystems,afterSystems);
            _serverClient = new DummyServer();
        }

        public void ChangeMap(Map map)
        {
            BaseSimulation.ChangeMap(map);
            _serverClient.ChangeMap(map);
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            _serverClient.CreatePlayer(input, transform);
            return BaseSimulation.CreatePlayer(input, transform);
        }

        public void Update(GameTime gameTime)
        {
            BaseSimulation.Update(gameTime);
            _serverClient.Update(gameTime);
        }
    }
}