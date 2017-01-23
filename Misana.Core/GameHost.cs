using System;
using System.Collections.Generic;
using Misana.Core.Communication;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core
{
    public class GameHost
    {
        private readonly INetworkClient client;
        private readonly List<BaseSystem> _beforSystems;
        private readonly List<BaseSystem> _afterSystems;

        private GameHostMode mode;

        public ConnectState ConnectionState { get; private set; }

        public GameHost(GameHostMode mode,INetworkClient client,List<BaseSystem> beforSystems,List<BaseSystem> afterSystems)
        {
            this.mode = mode;
            this.client = client;
            _beforSystems = beforSystems;
            _afterSystems = afterSystems;
        }

        
        public void Connect()
        {
            client.Connect();
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public ISimulation CreateWorld(string name)
        {
            if (mode == GameHostMode.Local && client.IsConnected)
            {
                return new SimulationClient(client,_beforSystems,_afterSystems);
            }
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}