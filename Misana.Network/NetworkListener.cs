using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Misana.Network
{
    public class NetworkListener
    {
        public const int PORT = 34560;

        private readonly BroadcastList<NetworkClient> _clients = new BroadcastList<NetworkClient>();

        private TcpListener _listener;
        private CancellationTokenSource _tokenSource;

        public NetworkListener()
        {
            _listener = new TcpListener(new IPEndPoint(IPAddress.Any,PORT));
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            _listener.Start();
            _listener.BeginAcceptTcpClient(TcpClientConnected, _tokenSource.Token);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _listener.Stop();
        }

        private void TcpClientConnected(IAsyncResult ar)
        {
            var token = (CancellationToken)ar.AsyncState;
            if (token.IsCancellationRequested)
                return;

            var tcpClient = _listener.EndAcceptTcpClient(ar);

            var networkClient = new NetworkClient(tcpClient);

            _clients.Add(networkClient);

            _listener.BeginAcceptTcpClient(TcpClientConnected, token);

            OnConnectClient(networkClient);
        }

        protected virtual void OnConnectClient(NetworkClient newClient)
        {

        }

        protected virtual void OnDisconnectClient(NetworkClient oldClient)
        {

        }
    }
}