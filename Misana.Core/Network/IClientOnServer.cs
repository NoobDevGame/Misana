using System;
using System.Net;

namespace Misana.Core.Network {
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
}