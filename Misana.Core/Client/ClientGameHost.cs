using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Misana.Core.Communication;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;
using Misana.Core.Network;
using Misana.Core.Systems;

namespace Misana.Core.Client
{
    public partial class ClientGameHost : ISimulation
    {
        private readonly List<BaseSystem> _beforeSystems;
        private readonly List<BaseSystem> _afterSystems;

        public ISimulation Simulation { get; private set; }
        public IServerOnClient Server { get; set; }

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


        public bool IsConnected { get; private set; }


        public ClientGameHost(List<BaseSystem> beforeSystems, List<BaseSystem> afterSystems)
        {
            _beforeSystems = beforeSystems;
            _afterSystems = afterSystems;
        }


        public async Task ChangeMap(Map map)
        {
            if (IsConnected)
            {
                var message = new ChangeMapMessageRequest(map.Name);
                var respone = await Query<ChangeMapMessageRequest, ChangeMapMessageResponse>(message);
                if (!respone.Result)
                    throw new NotSupportedException();
            }

            await Simulation.ChangeMap(map);
        }

        public async Task<int> Connect(string name,IPAddress address)
        {
            await Server.Connect(address);

            var responseMessage = await Query<LoginMessageRequest,LoginMessageResponse>(new LoginMessageRequest(name));

            IsConnected = true;
            return responseMessage.Id;
        }

        public void Disconnect()
        {
            Server.Disconnect();
            IsConnected = false;
        }

        public async Task CreateWorld(string name)
        {
            ISimulation simulation = null;

            if (!Server.IsConnected)
                throw new InvalidOperationException();

            var responseMessage = await Query<CreateWorldMessageRequest, CreateWorldMessageResponse>(new CreateWorldMessageRequest(name));

            if (!responseMessage.Result)
                throw new NotSupportedException();

            Simulation = CreateNetworkSimulation(responseMessage.FirstLocalId);
        }

        private ISimulation CreateNetworkSimulation(int firstLocalId)
        {
            List<BaseSystem> beforeSystems = new List<BaseSystem>();
            if (_beforeSystems != null)
                beforeSystems.AddRange(_beforeSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            if (_afterSystems != null)
                afterSystems.AddRange(_afterSystems);
            afterSystems.Add(new SendEntityPositionSystem());


            var simulation = new Simulation(SimulationMode.Local, beforeSystems, afterSystems, Server, firstLocalId);
            return simulation;
        }

        public void Update(GameTime gameTime)
        {
//            {
//                OnCreateEntityMessage message = new OnCreateEntityMessage();
//                while (Receiver.TryGetMessage(out message))
//                {
//                    Simulation.CreateEntity(message.DefinitionId, message.EntityId, null, null);
//                }
//            }


            if (Simulation != null && Simulation.State == SimulationState.Running)
            {
                IGameMessage msg;
                while (Server.TryDequeue(out msg))
                {
                    msg.ApplyOnClient(this);
                }

                Simulation?.Update(gameTime);
            }

            Server.FlushQueue();
        }

        public async Task Start()
        {
            if (IsConnected)
            {
                var message = new StartSimulationMessageRequest();
                var response = await Query<StartSimulationMessageRequest, StartSimulationMessageResponse>(message);

                if (!response.Result)
                    throw new NotSupportedException();
            }



            await  Simulation.Start();
        }

//        public Task<int> CreatePlayer(PlayerInputComponent playerInput, TransformComponent playerTransform)
//        {
//
//            return CreateEntity("Player", b =>
//            {
//                var transfrom = b.Get<TransformComponent>();
//                ComponentRegistry.Copy[ComponentRegistry<TransformComponent>.Index](transfrom, playerTransform);
//                b.Add(playerTransform);
//                b.Add(playerInput);
//                b.Add<SendComponent>();
//            }, null);
//        }

        public async Task JoinWorld(int id)
        {
            if (!IsConnected)
                throw new InvalidOperationException();

            JoinWorldMessageRequest messageRequest= new JoinWorldMessageRequest(id);
            var respone = await Query<JoinWorldMessageRequest, JoinWorldMessageResponse>(messageRequest);

            Simulation = CreateNetworkSimulation(respone.FirstLocalEntityId);

            if (respone.HaveWorld)
            {
                var map = MapLoader.Load(respone.MapName);

                await Simulation.ChangeMap(map);
            }
        }

        private async Task<TResponse> Query<TRequest, TResponse>(TRequest messageRequest)
            where TRequest : NetworkRequest
            where TResponse : NetworkResponse
        {
            ClientMessageHelper<TResponse>.Semaphore = new SemaphoreSlim(0);

            Server.Request(messageRequest);

            await ClientMessageHelper<TResponse>.Semaphore.WaitAsync();
            ClientMessageHelper<TResponse>.Semaphore.Dispose();
            ClientMessageHelper<TResponse>.Semaphore = null;
            return ClientMessageHelper<TResponse>.LastResult;
        }

        public SpawnerSystem SpawnerSystem => Simulation.SpawnerSystem;

    }

    internal class ClientMessageHelper<T> where T : RpcMessage
    {
        public static SemaphoreSlim Semaphore;
        public static T LastResult;
    }
}