using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Misana.Core.Client;
using Misana.Core.Server;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public class InternalNetworkClient : IClientOnServer, IServerOnClient
    {
        public InternalClientOnServer ClientOnServer;
        public InternalServerOnClient ServerOnClient;

        public InternalNetworkClient(ServerGameHost sgh, ClientGameHost cgh)
        {
            ClientOnServer = new InternalClientOnServer(this, sgh);
            ServerOnClient = new InternalServerOnClient(this, cgh);
        }

        void IOutgoingMessageQueue.Enqueue<T>(T msg)
        {
            ClientOnServer.Enqueue(msg);
        }

        int IClientOnServer.NetworkId => ClientOnServer.NetworkId;

        EndPoint IClientOnServer.UdpEndpoint => ClientOnServer.UdpEndpoint;

        void IClientOnServer.Respond<T>(T response, byte messageId)
        {
            ClientOnServer.Respond(response, messageId);
        }

        void IServerOnClient.FlushQueue()
        {
            ServerOnClient.FlushQueue();
        }

        void IServerOnClient.HandleData(byte[] udpBuffer, ref int processed)
        {
            ServerOnClient.HandleData(udpBuffer, ref processed);
        }

        Task IServerOnClient.Connect(IPAddress address)
        {
            return ServerOnClient.Connect(address);
        }

        void IServerOnClient.Disconnect()
        {
            ServerOnClient.Disconnect();
        }

        bool IServerOnClient.IsConnected => ServerOnClient.IsConnected;

        public IClientRpcMessageHandler ClientRpcHandler
        {
            set { ServerOnClient.ClientRpcHandler = value; }
        }

        public bool TryDequeue(out IGameMessage msg)
        {
            return ServerOnClient.TryDequeue(out msg);
        }

        void IServerOnClient.Request<TRequest>(TRequest messageRequest)
        {
            ServerOnClient.Request(messageRequest);
        }

        bool IClientOnServer.TryDequeue(out IGameMessage msg)
        {
            return ClientOnServer.TryDequeue(out msg);
        }

        void IClientOnServer.FlushQueue()
        {
            ClientOnServer.FlushQueue();
        }

        void IClientOnServer.Start()
        {
            ClientOnServer.Start();
        }

        void IClientOnServer.HandleData(byte[] udpBuffer, ref int processed)
        {
            ClientOnServer.HandleData(udpBuffer, ref processed);
        }

        void IClientOnServer.Send<T>(T msg)
        {
            ClientOnServer.Send(msg);
        }
    }
}