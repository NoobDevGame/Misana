using System.Net;
using Misana.Core.Network;

namespace Misana.Core.Communication
{
    public class NetworkPlayer : IClientOnServer
    {
        public NetworkPlayer(string name, IClientOnServer client)
        {
            Name = name;
            Client = client;
        }

        public string Name { get; private set; }

        public IClientOnServer Client { get; private set; }

        public int NetworkId => Client.NetworkId;
        public EndPoint UdpEndpoint
        {
            get { return Client.UdpEndpoint; }
        }

        public void Respond<T>(T response, byte messageId) where T : NetworkResponse
        {
            Client.Respond(response, messageId);
        }

        public bool TryDequeue(out IGameMessage msg)
        {
            return Client.TryDequeue(out msg);
        }

        public void FlushQueue()
        {
            Client.FlushQueue();
        }

        public void Start()
        {
            Client.Start();
        }

        public void HandleData(byte[] udpBuffer, ref int processed)
        {
            Client.HandleData(udpBuffer, ref processed);
        }

        public void Send<T>(T msg) where T : RpcMessage
        {
            Client.Send(msg);
        }

        public NetworkSimulation Simulation { get; private set; }

        public void SetSimulation(NetworkSimulation simulation)
        {
            Simulation = simulation;
        }

        public void Enqueue<T>(T msg) where T : NetworkMessage, IGameMessage
        {
            Client.Enqueue(msg);
        }
    }
}