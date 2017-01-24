using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkPlayer
    {
        public NetworkPlayer(string name, NetworkClient client, int id)
        {
            Name = name;
            Client = client;
            Id = id;
        }

        public string Name { get; private set; }

        public NetworkClient Client { get; private set; }

        public int Id { get; private set; }

        public NetworkSimulation Simulation { get; private set; }

        public void SetSimulation(NetworkSimulation simulation)
        {
            Simulation = simulation;
        }
    }
}