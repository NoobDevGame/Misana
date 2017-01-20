using System.Collections.Generic;
using Misana.Core.Communication;
using Misana.Core.Communication.Systems;
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

        private DummyServer _serverClient;

        public SimulationClient(List<BaseSystem> baseBeforSystems,List<BaseSystem> baseAfterSystems)
        {
            _serverClient = new DummyServer();

            List<BaseSystem> beforSystems = new List<BaseSystem>();
            if (baseBeforSystems != null)
                beforSystems.AddRange(baseBeforSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new ClientPositionSystem(_serverClient));
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);



            BaseSimulation = new Simulation(beforSystems,afterSystems);

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