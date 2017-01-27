using System.Collections.Generic;
using System.Linq;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
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

        protected override void OnConnectClient(NetworkClient newClient)
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
        }

        private void OnGetOuterPlayers(GetOuterPlayersMessageRequest message, MessageHeader header, NetworkClient client)
        {

            var simulation = players[client.ClientId].Simulation;
            foreach (var player in simulation.Players)
            {
                if (player.ClientId == client.ClientId)
                    continue;
                var response = new PlayerInfoMessage(player.ClientId, player.Name);
                client.SendMessage(ref response);
            }
        }

        private void OnJoinWorldRequest(JoinWorldMessageRequest message, MessageHeader header, NetworkClient client)
        {
            var simulation = simulations.FirstOrDefault(i => i.Id == message.Id);
            JoinWorldMessageResponse response = new JoinWorldMessageResponse(simulation != null
                ,simulation.BaseSimulation.CurrentMap != null,simulation.BaseSimulation.CurrentMap?.Name);

            if (simulation != null)
            {
                var player = players[client.ClientId];
                simulation.Players.Add(player);
                player.SetSimulation(simulation);

                OnJoinWorldMessage joinMessage = new OnJoinWorldMessage(player.ClientId,player.Name);
                simulation.Players.SendMessage(ref joinMessage,client.ClientId);

                OnGetOuterPlayers(default(GetOuterPlayersMessageRequest), default(MessageHeader), client);

            }

            client.SendResponseMessage(ref response,header.MessageId);

        }

        private void OnReadWorldsRequest(ReadWorldsMessageRequest message, MessageHeader header, NetworkClient client)
        {
            foreach (var simulation in simulations)
            {
                WorldInformationMessage response = new WorldInformationMessage(simulation.Id,simulation.Name);
                client.SendMessage(ref response);
            }
        }

        private void OnBroadcast<T>(T message, MessageHeader header, NetworkClient client)
            where T : struct
        {
            var simulation = players[client.ClientId].Simulation;
            simulation.Players.SendMessage(ref message);
        }

        private void OnNoOwnerBroadcast<T>(T message, MessageHeader header, NetworkClient client)
            where T : struct
        {
            var simulation = players[client.ClientId].Simulation;
            simulation.Players.SendMessage(ref message,client.ClientId);
        }

        protected override void OnDisconnectClient(NetworkClient oldClient)
        {
            base.OnDisconnectClient(oldClient);
        }

        private void OnStartRequest(StartSimulationMessageRequest message, MessageHeader header, NetworkClient networkClient)
        {
            var response = new StartSimulationMessageResponse(true);

            var simulation = players[networkClient.ClientId].Simulation;

            if (simulation.Owner.ClientId != networkClient.ClientId)
            {
                response.Result = false;
                networkClient.SendResponseMessage(ref response,header.MessageId);
                return;
            }

            simulation.BaseSimulation.Start();

            networkClient.SendResponseMessage(ref response,header.MessageId);

        }

        private void OnChangeMapRequest(ChangeMapMessageRequest message, MessageHeader header, NetworkClient networkClient)
        {
            ChangeMapMessageResponse response = new ChangeMapMessageResponse(true);

            var simulation = players[networkClient.ClientId].Simulation;

            if (simulation.Owner.ClientId != networkClient.ClientId)
            {
                response.Result = false;
                networkClient.SendResponseMessage(ref response,header.MessageId);
                return;
            }

            var map = MapLoader.Load(message.Name);

            simulation.BaseSimulation.ChangeMap(map);


            networkClient.SendResponseMessage(ref response,header.MessageId);
        }

        private async void OnCreateEntityRequest(CreateEntityMessageRequest message, MessageHeader header, NetworkClient networkClient)
        {
            var simulation = players[networkClient.ClientId].Simulation;
            var id = await simulation.BaseSimulation.CreateEntity(message.DefinitionId,null, null);

            var responseMessage = new CreateEntityMessageResponse(true,id);
            networkClient.SendResponseMessage(ref responseMessage,header.MessageId);

            var callbackmessage = new OnCreateEntityMessage(id,message.DefinitionId);
            simulation.Players.SendMessage(ref callbackmessage,networkClient.ClientId);

        }

        protected virtual void OnLoginRequest(LoginMessageRequest message, MessageHeader header,NetworkClient client)
        {
            var responseMessage = new LoginMessageResponse(client.ClientId);

            players.Add(client.ClientId,new NetworkPlayer(message.Name,client));

            client.SendResponseMessage(ref responseMessage,header.MessageId);

            OnReadWorldsRequest(default(ReadWorldsMessageRequest),default(MessageHeader), client);
        }

        protected virtual void OnCreateWorld(CreateWorldMessageRequest message,MessageHeader header,NetworkClient client)
        {
            var networkPlayer = players[client.ClientId];

            var simulation = new NetworkSimulation(networkPlayer,client,null,null);
            simulation.Name = message.Name;
            networkPlayer.SetSimulation(simulation);
            simulations.Add(simulation);

            CreateWorldMessageResponse messageResponse = new CreateWorldMessageResponse(true,simulation.Id );
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