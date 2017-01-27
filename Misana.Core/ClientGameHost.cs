using System;
using System.Collections.Generic;
using System.Net;
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
        public INetworkReceiver Receiver { get; private set; }
        public INetworkSender Sender { get; private set; }

        public bool IsConnected { get; private set; }


        public ClientGameHost(NetworkClient client, List<BaseSystem> beforSystems, List<BaseSystem> afterSystems)
        {
            this.client = client;
            _beforSystems = beforSystems;
            _afterSystems = afterSystems;
            Receiver = new EmptyNetworkReceive();
            Sender = new EmptyNetworkSender();
        }

        public async Task<int> Connect(string name,IPAddress address)
        {

            await client.Connect(new IPEndPoint(address,NetworkListener.PORT));


            Receiver = client;
            Sender = client;
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

        public async Task<ISimulation> CreateWorld(string name)
        {
            ISimulation simulation = null;

            if (client.IsConnected)
            {
                CreateWorldMessageRequest message = new CreateWorldMessageRequest(name);
                var waitobject = client.SendRequestMessage(ref message);

                var responseMessage = await waitobject.Wait<CreateWorldMessageResponse>();

                if (!responseMessage.Result)
                    throw  new NotSupportedException();


                simulation =  new SimulationClient(client, client,_beforSystems,_afterSystems);
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

        public async Task<ISimulation> JoinWorld(int id)
        {
            if (!IsConnected)
                throw new InvalidOperationException();

            JoinWorldMessageRequest messageRequest= new JoinWorldMessageRequest(id);
            var respone = await Sender.SendRequestMessage<JoinWorldMessageRequest>(ref messageRequest).Wait<JoinWorldMessageResponse>();

            Simulation =  new SimulationClient(client, client,_beforSystems,_afterSystems);

            return Simulation;
        }
    }
}