using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
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

        private NetworkClient _client;
        private int entiytIndex;
        private readonly int maxEntityIndex;


        public SimulationClient(NetworkClient client,
            int entiytIndex, int entityCount,
            List<BaseSystem> baseBeforSystems,List<BaseSystem> baseAfterSystems)
        {
            _client = client;
            this.entiytIndex = entiytIndex;
            maxEntityIndex = entityCount + entiytIndex;

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

        public async Task ChangeMap(Map map)
        {
            var message = new ChangeMapMessageRequest(map.Name);
            var respone = await _client.SendMessage(ref message).Wait<ChangeMapMessageResponse>();
            if (!respone.Result)
                throw new NotSupportedException();

             await BaseSimulation.ChangeMap(map);
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

        public async Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {

            var playerId = Interlocked.Increment(ref entiytIndex);

            var definition = CurrentMap.GlobalEntityDefinitions["Player"];

            var message = new CreateEntityMessageRequest(playerId,definition.Id);
            var response = await _client.SendMessage(ref message).Wait<CreateEntityMessageResponse>();

            if (!response.Result)
                throw new NotSupportedException();

             await BaseSimulation.CreatePlayer(input, transform,playerId);

            return playerId;
        }

        public Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform, int playerId)
        {
            throw new NotSupportedException();
        }

        public async Task Start()
        {
            var message = new StartSimulationMessageRequest();
            var response = await _client.SendMessage(ref message).Wait<StartSimulationMessageResponse>();

            if (!response.Result)
                throw new NotSupportedException();

            await BaseSimulation.Start();
        }

        public void CreateEntity(int defintionId, int entityId)
        {
            throw new NotSupportedException();
        }

        public void Update(GameTime gameTime)
        {
            BaseSimulation.Update(gameTime);
        }


    }
}