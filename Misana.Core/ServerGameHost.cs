using System.Collections.Generic;
using System.Linq;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core
{
    public class ServerGameHost : NetworkListener
    {


        private List<NetworkSimulation> simulations  = new List<NetworkSimulation>();

        //TODO: Dictionary weg
        protected Dictionary<int,NetworkPlayer> players = new Dictionary<int, NetworkPlayer>();
        public IEnumerable<NetworkPlayer> Players => players.Select(i => i.Value);

        public ServerGameHost()
        {

        }

        protected override void OnConnectClient(INetworkClient newClient)
        {
            newClient.RegisterOnMessageCallback<LoginMessageRequest>(OnLoginRequest);
            newClient.RegisterOnMessageCallback<CreateWorldMessageRequest>(OnCreateWorld);
            newClient.RegisterOnMessageCallback<CreateEntityMessageRequest>(OnCreateEntityRequest);
            newClient.RegisterOnMessageCallback<ChangeMapMessageRequest>(OnChangeMapRequest);
            newClient.RegisterOnMessageCallback<StartSimulationMessageRequest>(OnStartRequest);

            newClient.RegisterOnMessageCallback<ReadWorldsMessageRequest>(OnReadWorldsRequest);
            newClient.RegisterOnMessageCallback<GetOuterPlayersMessageRequest>(OnGetOuterPlayers);

            newClient.RegisterOnMessageCallback<JoinWorldMessageRequest>(OnJoinWorldRequest);

            newClient.RegisterOnMessageCallback<OnDropWieldedEffectMessage>(OnNoOwnerBroadcast);
            newClient.RegisterOnMessageCallback<OnPickupEffectMessage>(OnNoOwnerBroadcast);
            newClient.RegisterOnMessageCallback<EntityPositionMessage>(OnNoOwnerBroadcast);
            newClient.RegisterOnMessageCallback<OnCreateProjectileEffectMessage>(OnNoOwnerBroadcast);
            newClient.RegisterOnMessageCallback<SpawnerTriggeredMessage>(OnSpawnerTriggered);
        }

        private void OnSpawnerTriggered(SpawnerTriggeredMessage message, MessageHeader header, INetworkClient client)
        {
            if (players.ContainsKey(client.NetworkId))
            {
                var simulation = players[client.NetworkId].Simulation;
                //simulation.Players.ReceiveMessage(ref message, header, client);

                var owner = simulation.BaseSimulation.Entities.GetEntityById(message.SpawnerOwnerId);
                var spawnerComponent = owner.Get<SpawnerComponent>();

                var tf = ComponentRegistry<TransformComponent>.Take();
                tf.CurrentArea = simulation.BaseSimulation.CurrentMap.GetAreaById(message.AreaId);
                tf.Position = message.Position;
                tf.Radius = message.Radius;


                if (simulation.BaseSimulation.SpawnerSystem.CanSpawn(spawnerComponent, owner.Get<TransformComponent>()))
                {
                    ProjectileComponent pc = null;
                    if (message.Projectile)
                    {
                        pc = ComponentRegistry<ProjectileComponent>.Take();
                        pc.Move = message.Move;
                    }

                    simulation.BaseSimulation.SpawnerSystem.SpawnRemote(spawnerComponent, message.SpawnedEntityId, tf, pc);
                    simulation.Players.SendMessage(ref message, client.NetworkId);
                }
            }
        }

        private void OnGetOuterPlayers(GetOuterPlayersMessageRequest message, MessageHeader header, INetworkClient client)
        {

            var simulation = players[client.NetworkId].Simulation;
            foreach (var player in simulation.Players)
            {
                if (player.NetworkId == client.NetworkId)
                    continue;
                var response = new PlayerInfoMessage(player.NetworkId, player.Name);
                client.SendMessage(ref response);
            }
        }

        private void OnJoinWorldRequest(JoinWorldMessageRequest message, MessageHeader header, INetworkClient client)
        {
            var simulation = simulations.FirstOrDefault(i => i.Id == message.Id);
            JoinWorldMessageResponse response = new JoinWorldMessageResponse(simulation != null
                ,simulation.BaseSimulation.CurrentMap != null,simulation.BaseSimulation.CurrentMap?.Name, 50001 + simulation.Players.Count * 50000);

            if (simulation != null)
            {
                var player = players[client.NetworkId];
                simulation.Players.Add(player);
                player.SetSimulation(simulation);

                OnJoinWorldMessage joinMessage = new OnJoinWorldMessage(player.NetworkId,player.Name);
                simulation.Players.SendMessage(ref joinMessage,client.NetworkId);

                OnGetOuterPlayers(default(GetOuterPlayersMessageRequest), default(MessageHeader), client);

            }

            client.SendResponseMessage(ref response,header.MessageId);

        }

        private void OnReadWorldsRequest(ReadWorldsMessageRequest message, MessageHeader header, INetworkClient client)
        {
            foreach (var simulation in simulations)
            {
                WorldInformationMessage response = new WorldInformationMessage(simulation.Id,simulation.Name);
                client.SendMessage(ref response);
            }
        }

        private void OnBroadcast<T>(T message, MessageHeader header, INetworkClient client)
            where T : struct
        {
            var simulation = players[client.NetworkId].Simulation;
            simulation.Players.ReceiveMessage(ref message,header,client);
            simulation.Players.SendMessage(ref message);
        }

        private void OnNoOwnerBroadcast<T>(T message, MessageHeader header, INetworkClient client)
            where T : struct
        {
            if (players.ContainsKey(client.NetworkId))
            {
                var simulation = players[client.NetworkId].Simulation;
                simulation.Players.ReceiveMessage(ref message,header,client);
                simulation.Players.SendMessage(ref message,client.NetworkId);
            }
        }

        protected override void OnDisconnectClient(INetworkClient oldClient)
        {
            base.OnDisconnectClient(oldClient);
        }

        private void OnStartRequest(StartSimulationMessageRequest message, MessageHeader header, INetworkClient networkClient)
        {
            var response = new StartSimulationMessageResponse(true);

            var simulation = players[networkClient.NetworkId].Simulation;

            if (simulation.BaseSimulation.State == SimulationState.Running)
            {
                networkClient.SendResponseMessage(ref response,header.MessageId);
                return;
            }

            if (simulation.Owner.NetworkId != networkClient.NetworkId)
            {
                response.Result = false;
                networkClient.SendResponseMessage(ref response,header.MessageId);
                return;
            }

            simulation.BaseSimulation.Start();

            networkClient.SendResponseMessage(ref response,header.MessageId);
            OnStartSimulationMessage broodCastMessage = new OnStartSimulationMessage();
            simulation.Players.SendMessage(ref broodCastMessage,networkClient.NetworkId);

        }

        private void OnChangeMapRequest(ChangeMapMessageRequest message, MessageHeader header, INetworkClient networkClient)
        {
            ChangeMapMessageResponse response = new ChangeMapMessageResponse(true);

            var simulation = players[networkClient.NetworkId].Simulation;

            if (simulation.Owner.NetworkId != networkClient.NetworkId)
            {
                response.Result = false;
                networkClient.SendResponseMessage(ref response,header.MessageId);
                return;
            }

            var map = MapLoader.Load(message.Name);

            simulation.BaseSimulation.ChangeMap(map);


            networkClient.SendResponseMessage(ref response,header.MessageId);
        }

        private async void OnCreateEntityRequest(CreateEntityMessageRequest message, MessageHeader header, INetworkClient networkClient)
        {
            var simulation = players[networkClient.NetworkId].Simulation;
            var id = await simulation.BaseSimulation.CreateEntity(message.DefinitionId,null, null);

            var responseMessage = new CreateEntityMessageResponse(true,id);
            networkClient.SendResponseMessage(ref responseMessage,header.MessageId);

            var callbackmessage = new OnCreateEntityMessage(id,message.DefinitionId);
            simulation.Players.SendMessage(ref callbackmessage,networkClient.NetworkId);

        }

        protected virtual void OnLoginRequest(LoginMessageRequest message, MessageHeader header,INetworkClient client)
        {
            var responseMessage = new LoginMessageResponse(client.NetworkId);

            players.Add(client.NetworkId,new NetworkPlayer(message.Name,client));

            client.SendResponseMessage(ref responseMessage,header.MessageId);

            OnReadWorldsRequest(default(ReadWorldsMessageRequest),default(MessageHeader), client);
        }

        protected virtual void OnCreateWorld(CreateWorldMessageRequest message,MessageHeader header,INetworkClient client)
        {
            var networkPlayer = players[client.NetworkId];

            var simulation = new NetworkSimulation(networkPlayer,null,null);
            simulation.Name = message.Name;
            networkPlayer.SetSimulation(simulation);
            simulations.Add(simulation);

            CreateWorldMessageResponse messageResponse = new CreateWorldMessageResponse(true,simulation.Id, 50001);
            client.SendResponseMessage(ref messageResponse,header.MessageId);

        }



        //TODO: Entfernen
        public virtual void Update(GameTime gameTime)
        {
            foreach (var simulation in simulations)
            {
                simulation.BaseSimulation.Update(gameTime);
            }
        }
    }
}