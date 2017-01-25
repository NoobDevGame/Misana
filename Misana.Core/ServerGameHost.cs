using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core
{
    public class ServerGameHost
    {

        private readonly NetworkClient client;

        private System.Collections.Generic.List<NetworkSimulation> simulations  = new System.Collections.Generic.List<NetworkSimulation>();

        //TODO: Dictionary weg
        protected Dictionary<int,NetworkPlayer> players = new Dictionary<int, NetworkPlayer>();
        public IEnumerable<NetworkPlayer> Players => players.Select(i => i.Value);

        public ServerGameHost(NetworkClient client)
        {
            this.client = client;
            client.RegisterOnMessageCallback<LoginMessageRequest>(OnLoginRequest);
            client.RegisterOnMessageCallback<CreateWorldMessageRequest>(OnCreateWorld);
            client.RegisterOnMessageCallback<CreateEntityMessageRequest>(OnCreateEntityRequest);
            client.RegisterOnMessageCallback<ChangeMapMessageRequest>(OnChangeMapRequest);
            client.RegisterOnMessageCallback<StartSimulationMessageRequest>(OnStartRequest);
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

        private void OnCreateEntityRequest(CreateEntityMessageRequest message, MessageHeader header, NetworkClient networkClient)
        {
            var simulation = players[networkClient.ClientId].Simulation;
            simulation.BaseSimulation.CreateEntity(message.DefinitionId, message.EntityId);

            var responseMessage = new CreateEntityMessageResponse(true);
            networkClient.SendResponseMessage(ref responseMessage,header.MessageId);

        }

        protected virtual void OnLoginRequest(LoginMessageRequest message, MessageHeader header,NetworkClient client)
        {
            var responseMessage = new LoginMessageResponse(client.ClientId);

            players.Add(client.ClientId,new NetworkPlayer(message.Name,client));

            client.SendResponseMessage(ref responseMessage,header.MessageId);
        }

        protected virtual void OnCreateWorld(CreateWorldMessageRequest message,MessageHeader header,NetworkClient client)
        {
            var networkPlayer = players[client.ClientId];


            var simulation = new NetworkSimulation(networkPlayer,this.client,null,null);
            networkPlayer.SetSimulation(simulation);
            simulations.Add(simulation);

            var startIndex = (simulation.Players.Count + 1) * short.MaxValue;
            var entityCount = short.MaxValue;

            CreateWorldMessageResponse messageResponse = new CreateWorldMessageResponse(true,startIndex,entityCount );
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