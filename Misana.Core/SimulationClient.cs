using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Communication.Systems;
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

        public SimulationMode Mode => BaseSimulation.Mode;

        public NetworkEffectMessenger EffectMessenger => BaseSimulation.EffectMessenger;

        private INetworkSender _sender;
        private INetworkReceiver _receiver;


        public SimulationClient(INetworkSender sender, INetworkReceiver receiver,
             List<BaseSystem> baseBeforSystems, List<BaseSystem> baseAfterSystems)
        {
            _sender = sender;
            _receiver = receiver;

            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ReceiveEntityPositionSystem(_receiver));
            if (baseBeforSystems != null)
                beforSystems.AddRange(baseBeforSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(_sender));
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);

            BaseSimulation = new Simulation(SimulationMode.Local,beforSystems,afterSystems,sender,receiver);

            _receiver.RegisterOnMessageCallback<JoinWorldMessageResponse>(OnJoinWorld);
            _receiver.RegisterOnMessageCallback< OnStartSimulationMessage>(OnStartSimulation);
        }

        private void OnStartSimulation(OnStartSimulationMessage message, MessageHeader header, NetworkClient client)
        {
            BaseSimulation.Start();
        }

        private void OnJoinWorld(JoinWorldMessageResponse message, MessageHeader header, NetworkClient client)
        {
            if (message.HaveWorld)
            {
                var map = MapLoader.Load(message.MapName);
                BaseSimulation.ChangeMap(map);
            }
        }

        public async Task ChangeMap(Map map)
        {
            var message = new ChangeMapMessageRequest(map.Name);
            var respone = await _sender.SendRequestMessage(ref message).Wait<ChangeMapMessageResponse>();
            if (!respone.Result)
                throw new NotSupportedException();

             await BaseSimulation.ChangeMap(map);
        }

        public Task<int> CreateEntity(string definitionName, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var definition = CurrentMap.GlobalEntityDefinitions[definitionName];
            return CreateEntity(definition, createCallback, createdCallback);
        }

        public Task<int> CreateEntity(string definitionName, int entityId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var definition = CurrentMap.GlobalEntityDefinitions[definitionName];
            return CreateEntity(definition.Id,entityId, createCallback, createdCallback);
        }

        public Task<int> CreateEntity(EntityDefinition definition, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            return CreateEntity(definition.Id, createCallback, createdCallback);
        }

        public async Task<int> CreateEntity(int defintionId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            CreateEntityMessageRequest request = new CreateEntityMessageRequest(defintionId);
            var response = await _sender.SendRequestMessage(ref request).Wait<CreateEntityMessageResponse>();
            if (!response.Result)
                throw new InvalidOperationException();

            return await BaseSimulation.CreateEntity(defintionId, response.EntityId, createCallback, createdCallback);
        }

        public Task<int> CreateEntity(int defintionId, int entityId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            throw new NotImplementedException();
        }



        public async Task Start()
        {
            var message = new StartSimulationMessageRequest();
            var response = await _sender.SendRequestMessage(ref message).Wait<StartSimulationMessageResponse>();

            if (!response.Result)
                throw new NotSupportedException();

            await BaseSimulation.Start();
        }

        public void Update(GameTime gameTime)
        {
            BaseSimulation.Update(gameTime);
        }


    }
}