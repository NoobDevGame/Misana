using System;
using System.Net;
using System.Threading.Tasks;
using Misana.Core.Client;

namespace Misana.Core.Network {

    public class ServerGameMessage
    {
        public IGameMessage Message;
        public IClientOnServer Sender;
    }

    public interface IClientOnServer : IOutgoingMessageQueue
    {
        int NetworkId { get; }
        EndPoint UdpEndpoint { get; }
        void Respond<T>(T response, byte messageId) where T : NetworkResponse;

        bool TryDequeue(out IGameMessage msg);
        void FlushQueue();
        void Start();
        void HandleData(byte[] data, ref int processed);
        void Send<T>(T msg) where T : RpcMessage;
    }

    public interface IOutgoingMessageQueue
    {
        void Enqueue<T>(T msg) where T : NetworkMessage, IGameMessage;
    }

    public interface IServerOnClient : IOutgoingMessageQueue
    {
        void FlushQueue();
        void HandleData(byte[] data, ref int processed);
        Task Connect(IPAddress address);
        void Disconnect();
        bool IsConnected { get; }
        IClientRpcMessageHandler ClientRpcHandler { set; }
        bool TryDequeue(out IGameMessage msg);
        void Request<TRequest>(TRequest messageRequest) where TRequest : NetworkRequest;
    }
}