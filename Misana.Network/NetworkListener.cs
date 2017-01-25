using System.Collections.Generic;

namespace Misana.Network
{
    public class NetworkListener
    {
        private readonly BroadcastList<NetworkClient> _clients = new BroadcastList<NetworkClient>();

        public void Start()
        {

        }

        public void Stop()
        {

        }

        //TODO:Hier
        public NetworkClient NewDummyClient()
        {
            NetworkClient client = new NetworkClient(this);
            return client;
        }

        internal NetworkClient CreateClient(NetworkClient createClient)
        {
            NetworkClient client = new NetworkClient(createClient);
            _clients.Add(client);
            OnConnectClient(client);

            return client;
        }

        protected virtual void OnConnectClient(NetworkClient newClient)
        {

        }

        protected virtual void OnDisconnectClient(NetworkClient oldClient)
        {

        }
    }
}