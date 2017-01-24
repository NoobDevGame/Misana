using System.Collections.Generic;
using Misana.Core.Communication.Systems;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkSimulation
    {
        public NetworkPlayer Owner { get; private set; }

        public List<NetworkPlayer> Players { get;  private set; } = new List<NetworkPlayer>();

        public Simulation BaseSimulation { get; private set; }

        public NetworkSimulation(NetworkPlayer owner,NetworkClient client, List<BaseSystem> baseBeforSystems, List<BaseSystem> baseAfterSystems)
            : base()
        {
            Owner = owner;
            Players.Add(owner);

            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ReceiveEntityPositionSystem(client));
            if (baseBeforSystems != null)
                beforSystems.AddRange(baseBeforSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(client));
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);

            BaseSimulation = new Simulation(beforSystems,afterSystems);
        }
    }
}