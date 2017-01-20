using System.Collections.Generic;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class DummyServer : ISimulation , INetworkClient
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        public DummyServer()
        {
            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ServerEntityPositionSystem(this));

            List<BaseSystem> afterSystems = new List<BaseSystem>();

            BaseSimulation = new Simulation(beforSystems,null);
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

        public void SendMessageFast(NetworkMessage message)
        {

        }
    }
}