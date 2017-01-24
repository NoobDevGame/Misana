using System.Collections.Generic;
using Misana.Core.Ecs;

namespace Misana.Core.Communication
{
    public class NetworkSimulation : Simulation
    {
        public NetworkPlayer Owner { get; private set; }

        public List<NetworkPlayer> Players { get;  private set; } = new List<NetworkPlayer>();

        public NetworkSimulation(NetworkPlayer owner, List<BaseSystem> beforSystems, List<BaseSystem> afterSystems)
            : base(beforSystems, afterSystems)
        {
            Owner = owner;
            Players.Add(owner);
        }
    }
}