using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core
{
    public class ClientGameHost
    {
        private readonly NetworkClient client;
        private readonly List<BaseSystem> _beforSystems;
        private readonly List<BaseSystem> _afterSystems;

        private readonly List<ISimulation> _serverSimulations = new List<ISimulation>();

        public ISimulation Simulation { get; private set; }


        public ClientGameHost(NetworkClient client, List<BaseSystem> beforSystems, List<BaseSystem> afterSystems)
        {
            this.client = client;
            _beforSystems = beforSystems;
            _afterSystems = afterSystems;
        }

        public async Task Connect(string name)
        {

            await client.Connect();

            LoginMessageRequest message = new LoginMessageRequest(name);
            var responseMessage = await client.SendRequestMessage(ref message).Wait<LoginMessageResponse>();

        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public async Task<ISimulation> CreateWorld(string name)
        {
            ISimulation simulation = null;

            if (client.IsConnected)
            {
                CreateWorldMessageRequest message = new CreateWorldMessageRequest();
                var waitobject = client.SendRequestMessage(ref message);

                var responseMessage = await waitobject.Wait<CreateWorldMessageResponse>();

                if (!responseMessage.Result)
                    throw  new NotSupportedException();

                simulation =  new SimulationClient(client, client, responseMessage.EntityStartIndex,responseMessage.EntityStartIndex,_beforSystems,_afterSystems);
            }
            else
            {
                simulation = new Simulation(SimulationMode.SinglePlayer, _beforSystems,_afterSystems,new EmptyNetworkSender(), new EmptyNetworkReceive());
            }

            Simulation = simulation;

            return simulation;
        }

        public void Update(GameTime gameTime)
        {
            Simulation?.Update(gameTime);
        }

        public Task<int> CreatePlayer(PlayerInputComponent playerInput, TransformComponent playerTransform)
        {
            return Simulation.CreateEntity("Player", b =>
            {
                var transfrom = b.Get<TransformComponent>();
                transfrom.CopyTo(playerTransform);
                b.Add(playerTransform);
                b.Add(playerInput);
                b.Add<SendComponent>();
            }, null);
        }
    }
}