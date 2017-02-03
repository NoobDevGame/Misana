using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Misana.Core.Communication.Systems;
using Misana.Core.Ecs;
using Misana.Core.Network;

namespace Misana.Core.Communication
{
    public class NetworkSimulation
    {
        public NetworkPlayer Owner { get; private set; }

        public Simulation BaseSimulation { get; private set; }

        private static int _index;
        public List<NetworkPlayer> Players = new List<NetworkPlayer>();
        public int Id { get; } = Interlocked.Increment(ref _index);

        public string Name { get; set; }

        public NetworkSimulation(NetworkPlayer owner, List<BaseSystem> baseBeforeSystems, List<BaseSystem> baseAfterSystems, IOutgoingMessageQueue queue)
        {
            Owner = owner;

            List<BaseSystem> beforeSystems = new List<BaseSystem>();
            if (baseBeforeSystems != null)
                beforeSystems.AddRange(baseBeforeSystems);

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            if (baseAfterSystems != null)
                afterSystems.AddRange(baseAfterSystems);
            afterSystems.Add(new SendEntityPositionSystem());
            afterSystems.Add(new SendHealthSystem());


            BaseSimulation = new Simulation(SimulationMode.Server, beforeSystems,afterSystems, queue, 10001 );
        }
    }
}