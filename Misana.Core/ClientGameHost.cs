using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Misana.Core.Communication;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;
using Misana.Core.Systems;
using Misana.Network;

namespace Misana.Core
{
    public class ClientGameHost : ISimulation
    {
        private readonly INetworkClient client;
        private readonly List<BaseSystem> _beforeSystems;
        private readonly List<BaseSystem> _afterSystems;

        public ISimulation Simulation { get; private set; }
        public INetworkReceiver Receiver { get; private set; }
        public INetworkSender Sender { get; private set; }

        public Map CurrentMap => Simulation.CurrentMap;

        public EntityManager Entities => Simulation.Entities;

        public SimulationState State
        {
            get
            {
                if (Simulation != null)
                    return Simulation.State;
                return SimulationState.Unloaded;
            }
        }

        public SimulationMode Mode => Simulation.Mode;

        public EffectApplicator EffectMessenger => Simulation.EffectMessenger;

        public bool IsConnected { get; private set; }


        public ClientGameHost(INetworkClient client, List<BaseSystem> beforeSystems, List<BaseSystem> afterSystems)
        {
            this.client = client;
            _beforeSystems = beforeSystems;
            _afterSystems = afterSystems;
        }

        private void RegisterCallback()
        {
            Receiver.RegisterOnMessageCallback<JoinWorldMessageResponse>(OnJoinWorld);
            Receiver.RegisterOnMessageCallback<SpawnerTriggeredMessage>(Callback);
        }

        private void Callback(SpawnerTriggeredMessage message, MessageHeader header, INetworkClient client)
        {
            var simulation = Simulation;
            //simulation.Players.ReceiveMessage(ref message, header, client);

            if(Entities.GetEntityById(message.SpawnedEntityId) != null)
                return;

            var owner = simulation.Entities.GetEntityById(message.SpawnerOwnerId);
            var spawnerComponent = owner.Get<SpawnerComponent>();

            var tf = ComponentRegistry<TransformComponent>.Take();
            tf.CurrentArea = simulation.CurrentMap.GetAreaById(message.AreaId);
            tf.Position = message.Position;
            tf.Radius = message.Radius;


            if (SpawnerSystem.CanSpawn(spawnerComponent, owner.Get<TransformComponent>()))
            {
                ProjectileComponent pc = null;
                if (message.Projectile)
                {
                    pc = ComponentRegistry<ProjectileComponent>.Take();
                    pc.Move = message.Move;
                }

                SpawnerSystem.SpawnRemote(spawnerComponent, message.SpawnedEntityId, tf, pc);
             
            }
        }

        private void OnJoinWorld(JoinWorldMessageResponse message, MessageHeader header, INetworkClient client)
        {
            if (message.HaveWorld && Simulation != null)
            {
                var map = MapLoader.Load(message.MapName);
                Simulation.ChangeMap(map);
            }
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
            if (IsConnected)
            {
                CreateEntityMessageRequest request = new CreateEntityMessageRequest(defintionId);
                var response = await Sender.SendRequestMessage(ref request).Wait<CreateEntityMessageResponse>();
                if (!response.Result)
                    throw new InvalidOperationException();

                return await Simulation.CreateEntity(defintionId, response.EntityId, createCallback, createdCallback);
            }

            return await  Simulation.CreateEntity(defintionId, createCallback, createdCallback);
        }

        public Task<int> CreateEntity(int defintionId, int entityId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            throw new NotImplementedException();
        }

        public async Task ChangeMap(Map map)
        {
            if (IsConnected)
            {
                var message = new ChangeMapMessageRequest(map.Name);
                var respone = await Sender.SendRequestMessage(ref message).Wait<ChangeMapMessageResponse>();
                if (!respone.Result)
                    throw new NotSupportedException();
            }

            await Simulation.ChangeMap(map);
        }

        public async Task<int> Connect(string name,IPAddress address)
        {
            await client.Connect(address);

            Receiver = client;
            Sender = client;

            RegisterCallback();

            LoginMessageRequest message = new LoginMessageRequest(name);
            var responseMessage = await client.SendRequestMessage(ref message).Wait<LoginMessageResponse>();

            IsConnected = true;
            return responseMessage.Id;
        }

        public void Disconnect()
        {
            client.Disconnect();
            IsConnected = false;
        }

        public async Task CreateWorld(string name)
        {
            ISimulation simulation = null;

            if (!client.IsConnected)
                throw new InvalidOperationException();
            
            CreateWorldMessageRequest message = new CreateWorldMessageRequest(name);
            var waitobject = client.SendRequestMessage(ref message);

            var responseMessage = await waitobject.Wait<CreateWorldMessageResponse>();

            if (!responseMessage.Result)
                throw new NotSupportedException();

            Simulation = CreateNetworkSimulation(responseMessage.FirstLocalId);
        }

        private ISimulation CreateNetworkSimulation(int firstLocalId)
        {
            List<BaseSystem> beforeSystems = new List<BaseSystem>();
            beforeSystems.Add(new ReceiveEntityPositionSystem(Receiver));
            beforeSystems.Add(new ReceiveHealthSystem(Receiver));
            if (_beforeSystems != null)
                beforeSystems.AddRange(_beforeSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            if (_afterSystems != null)
                afterSystems.AddRange(_afterSystems);
            afterSystems.Add(new SendEntityPositionSystem(Sender));


            var simulation = new Simulation(SimulationMode.Local, beforeSystems, afterSystems, client, client, firstLocalId);
            return simulation;
        }

        public void Update(GameTime gameTime)
        {
            {
                OnCreateEntityMessage message = new OnCreateEntityMessage();
                while (Receiver.TryGetMessage(out message))
                {
                    Simulation.CreateEntity(message.DefinitionId, message.EntityId, null, null);
                }
            }

            Simulation?.Update(gameTime);
        }

        public async Task Start()
        {
            if (IsConnected)
            {
                var message = new StartSimulationMessageRequest();
                var response = await Sender.SendRequestMessage(ref message).Wait<StartSimulationMessageResponse>();

                if (!response.Result)
                    throw new NotSupportedException();
            }



            await  Simulation.Start();
        }

        public Task<int> CreatePlayer(PlayerInputComponent playerInput, TransformComponent playerTransform)
        {
            return CreateEntity("Player", b =>
            {
                var transfrom = b.Get<TransformComponent>();
                transfrom.CopyTo(playerTransform);
                b.Add(playerTransform);
                b.Add(playerInput);
                b.Add<SendComponent>();
            }, null);
        }

        public async Task JoinWorld(int id)
        {
            if (!IsConnected)
                throw new InvalidOperationException();

            JoinWorldMessageRequest messageRequest= new JoinWorldMessageRequest(id);
            var respone = await Sender.SendRequestMessage<JoinWorldMessageRequest>(ref messageRequest).Wait<JoinWorldMessageResponse>();

            Simulation = CreateNetworkSimulation(respone.FirstLocalEntityId);

            if (respone.HaveWorld)
            {
                var map = MapLoader.Load(respone.MapName);

                await Simulation.ChangeMap(map);
            }
        }

        public SpawnerSystem SpawnerSystem => Simulation.SpawnerSystem;
    }
}