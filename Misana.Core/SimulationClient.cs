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
    public class SimulationClient : ISimulation
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        public SimulationState State => BaseSimulation.State;


        private INetworkClient _client;


        public SimulationClient(INetworkClient client,List<BaseSystem> baseBeforSystems,List<BaseSystem> baseAfterSystems)
        {
            _client = client;

            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ReceiveEntityPositionSystem(_client));
            if (baseBeforSystems != null)
                beforSystems.AddRange(baseBeforSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(_client));
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);

            BaseSimulation = new Simulation(beforSystems,afterSystems);
        }

        public void ChangeMap(Map map)
        {
            //BaseSimulation.ChangeMap(map);
            //_server.ChangeMap(map);
        }

        public void CreateEntity(string definitionName)
        {
            //_server.CreateEntity(definitionName);
            //BaseSimulation.CreateEntity(definitionName);
        }

        public void CreateEntity(EntityDefinition defintion)
        {
            //_server.CreateEntity(defintion);
            //BaseSimulation.CreateEntity(defintion);
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            throw  new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            BaseSimulation.Update(gameTime);
        }

        /*
        public void CreateWorld(string name)
        {
            if (ConnectionState == ConnectState.Local)
            {
                BaseSimulation = new Simulation(baseBeforSystems,baseAfterSystems);
            }
            else
            {
                { //Server
                    List<BaseSystem> beforSystems = new List<BaseSystem>();
                    beforSystems.Add(new ReceiveEntityPositionSystem(_client.Server));
                    if (baseBeforSystems != null)
                        beforSystems.AddRange(baseBeforSystems);

                    List<BaseSystem> afterSystems = new List<BaseSystem>();
                    afterSystems.Add(new SendEntityPositionSystem(_client.Server));
                    if (baseAfterSystems != null)
                        afterSystems.AddRange(baseAfterSystems);

                    _serverSimulation = new Simulation(beforSystems,afterSystems);

                    _server = new NetworkWorld(_client.Server,_serverSimulation);
                }

                {//Client
                    List<BaseSystem> beforSystems = new List<BaseSystem>();
                    beforSystems.Add(new ReceiveEntityPositionSystem(_client));
                    if (baseBeforSystems != null)
                        beforSystems.AddRange(baseBeforSystems);

                    List<BaseSystem> afterSystems = new List<BaseSystem>();
                    afterSystems.Add(new SendEntityPositionSystem(_client));
                    if (baseAfterSystems != null)
                        afterSystems.AddRange(baseAfterSystems);

                    BaseSimulation = new Simulation(beforSystems,afterSystems);

                    _client = new NetworkWorld(_client,BaseSimulation);
                }
            }
        }
        */
    }
}