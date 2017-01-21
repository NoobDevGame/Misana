using System;
using System.Collections.Generic;
using Misana.Core.Communication;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;

namespace Misana.Core
{
    public class SimulationClient : ISimulation
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        public SimulationState State => BaseSimulation.State;

        private NetworkWorld _serverClient;

        public SimulationClient(List<BaseSystem> baseBeforSystems,List<BaseSystem> baseAfterSystems)
        {
            _serverClient = new NetworkWorld();

            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ReceiveEntityPositionSystem(_serverClient));
            if (baseBeforSystems != null)
                beforSystems.AddRange(baseBeforSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(_serverClient));
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);



            BaseSimulation = new Simulation(beforSystems,afterSystems);

        }

        public void ChangeMap(Map map)
        {
            BaseSimulation.ChangeMap(map);
            _serverClient.ChangeMap(map);
        }

        public int CreateEntity(string definitionName)
        {
            var serverId = _serverClient.CreateEntity(definitionName);
            var localId =  BaseSimulation.CreateEntity(definitionName);

            if (serverId != localId)
                throw new Exception("IDs sind nicht gleich");

            return serverId;
        }

        public int CreateEntity(EntityDefinition defintion)
        {
            var serverId = _serverClient.CreateEntity(defintion);
            var localId =  BaseSimulation.CreateEntity(defintion);

            if (serverId != localId)
                throw new Exception("IDs sind nicht gleich");

            return serverId;
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            var serverId = _serverClient.CreateEntity("Player");
            var localId = BaseSimulation.CreatePlayer(input, transform);

            if (serverId != localId)
                throw new Exception("IDs sind nicht gleich");

            return serverId;
        }

        public void Update(GameTime gameTime)
        {
            BaseSimulation.Update(gameTime);
            _serverClient.Update(gameTime);
        }
    }
}