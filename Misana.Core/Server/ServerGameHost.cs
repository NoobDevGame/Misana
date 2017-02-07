using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Misana.Core.Client;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Ecs;
using Misana.Core.Network;

namespace Misana.Core.Server
{
    public partial class ServerGameHost : IOutgoingMessageQueue
    {

        private readonly List<IClientOnServer> _clients = new List<IClientOnServer>();
        //private readonly BroadcastList<IClientOnServer> _clients = new BroadcastList<IClientOnServer>();
        private readonly Dictionary<IPAddress,IClientOnServer> _ipClients = new Dictionary<IPAddress, IClientOnServer>();

        private readonly TcpListener _listener;
        private CancellationTokenSource _tokenSource;


        public ServerGameHost()
        {
            _listener = new TcpListener(new IPEndPoint(IPAddress.Any, NetworkManager.TcpPort));

            _udpEndPoint = new IPEndPoint(IPAddress.Any, NetworkManager.ServerUdpPort);
        }

        public const int SIO_UDP_CONNRESET = -1744830452; // Ignore Udp Connection Reset
        public void StartListening()
        {
            _udpSendBuffer = new byte[1536];
            _tokenSource = new CancellationTokenSource();
            _listener.Start();
            _listener.BeginAcceptTcpClient(TcpClientConnected, _tokenSource.Token);
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, NetworkManager.ServerUdpPort));
            _udpClient.Client.IOControl(
                (IOControlCode)SIO_UDP_CONNRESET,
                new byte[] { 0, 0, 0, 0 },
                null
            );
            _udpWorker = new Thread(UdpWorkerLoop);
            _udpWorker.Start();
            _udpClient.Client.BeginReceiveFrom(_udpReadBuffer, 0, _udpReadBuffer.Length, SocketFlags.None, ref _udpEndPoint, OnUdpRead, null);
            //simulation = new NetworkSimulation()
        }

        public void StopListening()
        {
            if (_tokenSource == null)
                return;
            _tokenSource.Cancel();
            _listener.Stop();
        }

        public InternalNetworkClient CreateLocalClient(ClientGameHost cgh)
        {
            var client = new InternalNetworkClient(this, cgh);
            ((IClientOnServer) client).Start();
            _clients.Add(client.ClientOnServer);
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

                IClientOnServer networkClient = new ClientOnServer(this, tcpClient, ipAdress); //new NetworkClient(tcpClient,udpListener.UdpClient,ipAdress);



                _clients.Add(networkClient);
                _ipClients.Add(ipAdress,networkClient);

                _listener.BeginAcceptTcpClient(TcpClientConnected, token);
                networkClient.Start();

            }
            catch (Exception e)
            {
                tcpClient.Close();
                //Console.WriteLine(e);
                //throw;
            }


        }


        internal IClientOnServer GetClientByIp(IPAddress senderAddress)
        {
            return _ipClients[senderAddress];
        }

        private NetworkSimulation simulation;

        private EntityManager Entities => simulation.BaseSimulation.Entities;
        protected IntMap<NetworkPlayer> players = new IntMap<NetworkPlayer>(16);
        private bool _flushing;


        public void OnDisconnectClient(IClientOnServer oldClient)
        {
            _clients.Remove(oldClient);
            foreach(var kvp in _ipClients)
                if (kvp.Value == oldClient)
                {
                    _ipClients.Remove(kvp.Key);
                    break;
                }

            Broadcast(new PlayerLeftMessage {
                EntityIds = new List<int>(),
                PlayerId = oldClient.NetworkId
            },  oldClient.NetworkId);
        }



        public virtual void Update(GameTime gameTime)
        {
            if(simulation == null)
                return;

            foreach (var client in _clients)
            {
                IGameMessage msg;
                while (client.TryDequeue(out msg))
                {
                    msg.ApplyOnServer(this, client);
                }
            }

            simulation.BaseSimulation.Update(gameTime);
            foreach(var client in _clients)
                client.FlushQueue();

            _flushing = true;
            _udpWorkerResetEvent.Set();
        }



        public void Enqueue<T>(T msg) where T : NetworkMessage, IGameMessage
        {
            foreach(var client in _clients)
                client.Enqueue(msg);
        }

        private void Enqueue<T>(T msg, int excludeClientId) where T : NetworkMessage, IGameMessage
        {
            foreach(var client in _clients)
                if(client.NetworkId != excludeClientId)
                    client.Enqueue(msg);
        }

        private void Broadcast<T>(T msg, int excludeClientId) where T : RpcMessage
        {
            foreach(var client in _clients)
                if(client.NetworkId != excludeClientId)
                    client.Send(msg);
        }
    }
}