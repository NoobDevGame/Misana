using System;
using System.Collections.Generic;
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

        public GameHost(GameHostMode mode,NetworkClient client,List<BaseSystem> beforSystems,List<BaseSystem> afterSystems)
        {
            this.mode = mode;
            this.client = client;
            _beforSystems = beforSystems;
            _afterSystems = afterSystems;

            client.RegisterOnMessageCallback<CreateWorldRequestMessage>(OnCreateWorld);
        }

        private void OnCreateWorld(CreateWorldRequestMessage message,MessageHeader header)
        {
            CreateWorldResponeMessage responeMessage = new CreateWorldResponeMessage();
            client.SendMessage(ref responeMessage,header.MessageId);
        }


        public async Task Connect()
        {
            await client.Connect();
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public async Task<ISimulation> CreateWorld(string name)
        {
            if (mode == GameHostMode.Local && client.IsConnected)
            {
                CreateWorldRequestMessage message = new CreateWorldRequestMessage();
                var waitobject = client.SendMessage(ref message);

                var responseMessage = await waitobject.Wait<CreateWorldResponeMessage>();

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