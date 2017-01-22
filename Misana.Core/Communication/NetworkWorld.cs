using System.Collections.Generic;
using System.Linq;
using Misana.Core.Communication.Messages;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkWorld : INetworkWorld
    {
        public ConnectState ConnectionState { get; private set; }

        private ISimulation simulation;
        private INetworkClient client;
        public NetworkWorld(INetworkClient client,ISimulation simulation )
        {
            this.simulation = simulation;
            this.client = client;
            client.RegisterOnMessageCallback<CreateEntityMessage>(OnCreateEntity);
            
            client.RegisterOnMessageCallback<LoginRequestMessage>(OnLoginRequest);
            client.RegisterOnMessageCallback<LoginResponeMessage>(OnLoginResponse);
            
        }

        private void OnLoginResponse(LoginResponeMessage message)
        {
            ConnectionState = ConnectState.Connected;
        }

        private void OnLoginRequest(LoginRequestMessage message)
        {
            LoginResponeMessage responeMessage = new LoginResponeMessage();
            client.SendMessage(ref responeMessage);
        }

        private void OnCreateEntity(CreateEntityMessage message)
        {
            var definition = simulation.CurrentMap.GlobalEntityDefinitions.First(i => i.Value.Id == message.DefinitionId).Value;
            simulation.CreateEntity(definition);

        }


        public void Connect()
        {
            LoginRequestMessage message = new LoginRequestMessage();

            client.SendMessage(ref message);
        }

        public void Disconnect()
        {
        }
    }
}