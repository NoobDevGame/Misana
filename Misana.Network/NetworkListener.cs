using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Misana.Network
{
    public class NetworkListener
    {


        private readonly BroadcastList<INetworkClient> _clients = new BroadcastList<INetworkClient>();
        private readonly Dictionary<IPAddress,NetworkClient> _ipClients = new Dictionary<IPAddress, NetworkClient>();

        private readonly TcpListener _listener;
        private CancellationTokenSource _tokenSource;

        private UdpListnerClient udpListener;

        public NetworkListener()
        {
            _listener = new TcpListener(new IPEndPoint(IPAddress.Any, NetworkManager.TcpPort));

        }

        public async Task Start()
        {
            _tokenSource = new CancellationTokenSource();
            _listener.Start();
            _listener.BeginAcceptTcpClient(TcpClientConnected, _tokenSource.Token);

            udpListener = new UdpListnerClient(this);
            await udpListener.Connect(IPAddress.Any);
            OnConnectClient(udpListener);
        }

        public void Stop()
        {
            if (_tokenSource == null)
                return;
            _tokenSource.Cancel();
            _listener.Stop();
        }

        public INetworkClient CreateLocalClient()
        {
            var client = new InternalNetworkClient();
            _clients.Add(client.ServerClient);
            OnConnectClient(client.ServerClient);
            return client;
        }



        private void TcpClientConnected(IAsyncResult ar)
        {
            var token = (CancellationToken)ar.AsyncState;
            if (token.IsCancellationRequested)
                return;

            var tcpClient = _listener.EndAcceptTcpClient(ar);

            try
            {
                var ipAdress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;

                var networkClient = new NetworkClient(tcpClient,udpListener.UdpClient,ipAdress);

                _clients.Add(networkClient);
                _ipClients.Add(ipAdress,networkClient);

                _listener.BeginAcceptTcpClient(TcpClientConnected, token);
                OnConnectClient(networkClient);

                networkClient.StartRead();

            }
            catch (Exception e)
            {
                tcpClient.Close();
                //Console.WriteLine(e);
                //throw;
            }


        }

        protected virtual void OnConnectClient(INetworkClient newClient)
        {

        }

        protected virtual void OnDisconnectClient(INetworkClient oldClient)
        {

        }

        internal NetworkClient GetClientByIp(IPAddress senderAddress)
        {
            return _ipClients[senderAddress];
        }
    }
}