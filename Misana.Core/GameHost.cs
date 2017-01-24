using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core
{
    public class GameHost
    {
        private readonly NetworkClient client;
        private readonly List<BaseSystem> _beforSystems;
        private readonly List<BaseSystem> _afterSystems;

        private GameHostMode mode;

        public ConnectState ConnectionState { get; private set; }

        private int PlayerIndex = 0;

        public GameHost(GameHostMode mode,NetworkClient client,List<BaseSystem> beforSystems,List<BaseSystem> afterSystems)
        {
            this.mode = mode;
            this.client = client;
            _beforSystems = beforSystems;
            _afterSystems = afterSystems;

            client.RegisterOnMessageCallback<LoginRequestMessage>(OnLoginRequest);
            client.RegisterOnMessageCallback<CreateWorldRequestMessage>(OnCreateWorld);
        }

        private void OnLoginRequest(LoginRequestMessage message, MessageHeader header,NetworkClient client)
        {
            var playerIndex = Interlocked.Increment(ref PlayerIndex);
            var responseMessage = new LoginResponseMessage(playerIndex);
            client.SendMessage(ref responseMessage,header.MessageId);
        }

        private void OnCreateWorld(CreateWorldRequestMessage message,MessageHeader header,NetworkClient client)
        {
            CreateWorldResponseMessage responseMessage = new CreateWorldResponseMessage();
            client.SendMessage(ref responseMessage,header.MessageId);
        }


        public async Task Connect(string name)
        {
            if (mode == GameHostMode.Server || mode == GameHostMode.Hybrid)
                throw new NotSupportedException();

            await client.Connect();

            LoginRequestMessage message = new LoginRequestMessage(name);
            var responseMessage = await client.SendMessage(ref message).Wait<LoginResponseMessage>();

        }

        public void Disconnect()
        {
            if (mode == GameHostMode.Server|| mode == GameHostMode.Hybrid)
                throw new NotSupportedException();

            client.Disconnect();
        }

        public async Task<ISimulation> CreateWorld(string name)
        {
            if (mode == GameHostMode.Local && client.IsConnected)
            {
                CreateWorldRequestMessage message = new CreateWorldRequestMessage();
                var waitobject = client.SendMessage(ref message);

                var responseMessage = await waitobject.Wait<CreateWorldResponseMessage>();

                return new SimulationClient(client,_beforSystems,_afterSystems);
            }
            else if (mode == GameHostMode.Local && !client.IsConnected)
            {
                return new Simulation(_beforSystems,_afterSystems);
            }

            throw new NotSupportedException();
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}