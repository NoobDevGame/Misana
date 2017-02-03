using Misana.Core.Client;
using Misana.Core.Server;

namespace Misana.Core.Network {
    public class NetworkMessage
    {

    }

    public abstract class TcpMessage  : NetworkMessage {}

    public abstract class RpcMessage : TcpMessage, IRpcMessage
    {
        public abstract void HandleOnClient(IClientRpcMessageHandler h);
        public abstract void HandleOnServer(IServerRpcMessageHandler h,  byte messageId,IClientOnServer client);
    }

    public abstract class NetworkRequest : RpcMessage { }

    public abstract class NetworkResponse : RpcMessage {}

    public abstract class UdpMessage : NetworkMessage {}

    public abstract class TcpGameMessage : TcpMessage, IGameMessage
    {
        public abstract void ApplyOnClient(IClientGameMessageApplicator a);
        public abstract void ApplyOnServer(IServerGameMessageApplicator a, IClientOnServer client);
    }

    public abstract class UdpGameMessage : UdpMessage, IGameMessage
    {
        public abstract void ApplyOnClient(IClientGameMessageApplicator a);
        public abstract void ApplyOnServer(IServerGameMessageApplicator a, IClientOnServer client);
    }

    public interface IGameMessage
    {
        void ApplyOnClient(IClientGameMessageApplicator a);
        void ApplyOnServer(IServerGameMessageApplicator a, IClientOnServer client);
    }

    public interface IRpcMessage
    {
        void HandleOnClient(IClientRpcMessageHandler h);
        void HandleOnServer(IServerRpcMessageHandler h, byte messageId, IClientOnServer client);
    }
}