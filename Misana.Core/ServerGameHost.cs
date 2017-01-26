using System.Collections.Generic;
using System.Linq;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
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

            newClient.RegisterOnMessageCallback<DropWieldedMessage>(OnBroadcast);
        }

        private void OnBroadcast<T>(T message, MessageHeader header, NetworkClient client)
            where T : struct
        {
            var simulation = players[client.ClientId].Simulation;
            simulation.Players.SendMessage(ref message);
        }
        /*
        private void OnDropWielded(DropWieldedMessage message, MessageHeader header, NetworkClient client)
        {
            var simulation = players[client.ClientId].Simulation;
            var em = simulation.BaseSimulation.Entities;
            var owner = em.GetEntityById(message.OwnerId);
            var wielded = em.GetEntityById(message.WieldedId);

            if (wielded == null || owner == null)
                return;

            var ownerWielding = owner.Get<WieldingComponent>();
            var ownerTransform = owner.Get<TransformComponent>();

            if (ownerWielding == null || ownerTransform == null)
                return;

            if (ownerWielding.RightHandEntityId != message.WieldedId)
                return;

            var wieldedTransform = wielded.Get<TransformComponent>();

            if (wieldedTransform == null)
                return;

            wielded.Remove<WieldedComponent>().Add<DroppedItemComponent>();

            ownerWielding.TwoHanded = false;
            ownerWielding.RightHandEntityId = 0;

            wieldedTransform.Radius /= 2;
            wieldedTransform.ParentEntityId = 0;
            wieldedTransform.Position = ownerTransform.Position;

            simulation.Players.SendMessage(ref message);
        }
        */

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
        }

        protected virtual void OnCreateWorld(CreateWorldMessageRequest message,MessageHeader header,NetworkClient client)
        {
            var networkPlayer = players[client.ClientId];

            var simulation = new NetworkSimulation(networkPlayer,client,null,null);
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