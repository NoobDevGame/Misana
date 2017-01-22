using System;
using System.CodeDom;
using System.Collections.Generic;
using Misana.Core.Communication;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core
{
    public class SimulationClient : ISimulation , INetworkWorld
    {
        public ISimulation ClientSimulation { get; private set; }

        public Map CurrentMap => ClientSimulation.CurrentMap;

        public EntityManager Entities => ClientSimulation.Entities;

        public SimulationState State => ClientSimulation.State;

        public ConnectState ConnectionState => _client.ConnectionState;

        private NetworkWorld _server;
        private ISimulation _serverSimulation;
        private NetworkWorld _client;

        private InternNetworkClient _network = new InternNetworkClient();

        public SimulationClient(List<BaseSystem> baseBeforSystems,List<BaseSystem> baseAfterSystems)
        {
            { //Server
                List<BaseSystem> beforSystems = new List<BaseSystem>();
                beforSystems.Add(new ReceiveEntityPositionSystem(_network.Server));
                if (baseBeforSystems != null)
                    beforSystems.AddRange(baseBeforSystems);

                List<BaseSystem> afterSystems = new List<BaseSystem>();
                afterSystems.Add(new SendEntityPositionSystem(_network.Server));
                if (baseAfterSystems != null)
                    afterSystems.AddRange(baseAfterSystems);

                _serverSimulation = new Simulation(beforSystems,afterSystems);

                _server = new NetworkWorld(_network.Server,_serverSimulation);
            }

            {//Client
                List<BaseSystem> beforSystems = new List<BaseSystem>();
                beforSystems.Add(new ReceiveEntityPositionSystem(_network));
                if (baseBeforSystems != null)
                    beforSystems.AddRange(baseBeforSystems);

                List<BaseSystem> afterSystems = new List<BaseSystem>();
                afterSystems.Add(new SendEntityPositionSystem(_network));
                if (baseAfterSystems != null)
                    afterSystems.AddRange(baseAfterSystems);

               ClientSimulation = new Simulation(beforSystems,afterSystems);

                _client = new NetworkWorld(_network,ClientSimulation);
            }

            Connect();

        }

        public void ChangeMap(Map map)
        {
            //ClientSimulation.ChangeMap(map);
            //_server.ChangeMap(map);
        }

        public void CreateEntity(string definitionName)
        {
            //_server.CreateEntity(definitionName);
            //ClientSimulation.CreateEntity(definitionName);
        }

        public void CreateEntity(EntityDefinition defintion)
        {
            //_server.CreateEntity(defintion);
            //ClientSimulation.CreateEntity(defintion);
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            if (ConnectionState == ConnectState.Local)
            {
                return ClientSimulation.CreatePlayer(input, transform);
            }
            else
            {
                var playerId = ClientSimulation.CreatePlayer(input, transform);
                return playerId;
            }
        }

        public void Update(GameTime gameTime)
        {
            ClientSimulation.Update(gameTime);
           _serverSimulation.Update(gameTime);
        }

        public void Connect()
        {
            _client.Connect();
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}