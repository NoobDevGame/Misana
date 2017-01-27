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
using Misana.Core.Events;
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
        private int entiytIndex;
        private readonly int maxEntityIndex;


        public SimulationClient(INetworkSender sender, INetworkReceiver receiver,
            int entiytIndex, int entityCount, System.Collections.Generic.List<BaseSystem> baseBeforSystems, System.Collections.Generic.List<BaseSystem> baseAfterSystems)
        {
            _sender = sender;
            _receiver = receiver;
            this.entiytIndex = entiytIndex;
            maxEntityIndex = entityCount + entiytIndex;

            System.Collections.Generic.List<BaseSystem> beforSystems = new System.Collections.Generic.List<BaseSystem>();
            beforSystems.Add(new ReceiveEntityPositionSystem(_receiver));
            if (baseBeforSystems != null)
                beforSystems.AddRange(baseBeforSystems);

            System.Collections.Generic.List<BaseSystem> afterSystems = new System.Collections.Generic.List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(_sender));
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);

            BaseSimulation = new Simulation(SimulationMode.Local,beforSystems,afterSystems,sender,receiver);
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