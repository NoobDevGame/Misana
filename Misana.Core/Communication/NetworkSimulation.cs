using System.Collections.Generic;
using System.Threading;
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

        private static int _index;
        public int Id { get; } = Interlocked.Increment(ref _index);

        public string Name { get; set; }

        public NetworkSimulation(NetworkPlayer owner, List<BaseSystem> baseBeforeSystems, List<BaseSystem> baseAfterSystems)
        {
            Owner = owner;
            Players.Add(owner);

            List<BaseSystem> beforeSystems = new List<BaseSystem>();
            beforeSystems.Add(new ReceiveEntityPositionSystem(Players));
            if (baseBeforeSystems != null)
                beforeSystems.AddRange(baseBeforeSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);
            afterSystems.Add(new SendEntityPositionSystem(Players));
            afterSystems.Add(new SendHealthSystem(Players));


            BaseSimulation = new Simulation(SimulationMode.Server, beforeSystems,afterSystems,Players,Players);
        }
    }
}