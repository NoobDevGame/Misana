using System.Collections.Generic;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkWorld : InternNetworkClient, ISimulation
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        public SimulationState State => BaseSimulation.State;

        public NetworkWorld()
        {
            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ReceiveEntityPositionSystem(OuterClient));

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(OuterClient));

            BaseSimulation = new Simulation(beforSystems,afterSystems);
        }

        public void ChangeMap(Map map)
        {
            BaseSimulation.ChangeMap(map);
        }

        public int CreateEntity(string definitionName)
        {
            return BaseSimulation.CreateEntity(definitionName);
        }

        public int CreateEntity(EntityDefinition defintion)
        {
            return BaseSimulation.CreateEntity(defintion);
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