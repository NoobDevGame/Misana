using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkPlayer : INetworkSender , INetworkClient
    {
        public NetworkPlayer(string name, NetworkClient client)
        {
            Name = name;
            Client = client;
        }

        public string Name { get; private set; }

        public NetworkClient Client { get; private set; }

        public int ClientId => Client.ClientId;

        public NetworkSimulation Simulation { get; private set; }

        public void SetSimulation(NetworkSimulation simulation)
        {
            Simulation = simulation;
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            Client.SendMessage(ref message);
        }
    }
}