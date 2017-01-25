using System.Collections.Generic;
using Misana.Core.Communication.Systems;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkSimulation
    {
        public NetworkPlayer Owner { get; private set; }

        public BroadcastList<NetworkPlayer> Players { get; private set; } = new BroadcastList<NetworkPlayer>();

        public Simulation BaseSimulation { get; private set; }

        public NetworkSimulation(NetworkPlayer owner,NetworkClient client, List<BaseSystem> baseBeforSystems, List<BaseSystem> baseAfterSystems)
        {
            Owner = owner;
            Players.Add(owner);

            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ServerReceiveEntityPositionSystem(Players,Players));
            if (baseBeforSystems != null)
                beforSystems.AddRange(baseBeforSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(client));
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);

            BaseSimulation = new Simulation(SimulationMode.Server, beforSystems,afterSystems);
        }
    }
}