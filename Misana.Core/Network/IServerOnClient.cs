using System.Net;
using System.Threading.Tasks;
using Misana.Core.Client;

namespace Misana.Core.Network {
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