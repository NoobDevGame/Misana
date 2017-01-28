using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkPlayer : INetworkSender ,INetworkReceiver, INetworkIdentifier
    {
        public NetworkPlayer(string name, NetworkClient client)
        {
            Name = name;
            Client = client;
        }

        public string Name { get; private set; }

        public NetworkClient Client { get; private set; }

        public int NetworkId => Client.NetworkId;

        public NetworkSimulation Simulation { get; private set; }

        public void SetSimulation(NetworkSimulation simulation)
        {
            Simulation = simulation;
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            Client.SendMessage(ref message);
        }

        public MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct
        {
            return Client.SendRequestMessage(ref message);
        }

        public void SendResponseMessage<T>(ref T message, byte messageid) where T : struct
        {
            Client.SendResponseMessage(ref message,messageid);
        }

        public bool TryGetMessage<T>(out T message, out INetworkIdentifier senderClient) where T : struct
        {
            return Client.TryGetMessage(out message, out senderClient);
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            return Client.TryGetMessage<T>(out message);
        }

        public void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback) where T : struct
        {
            Client.RegisterOnMessageCallback(callback);
        }
    }
}