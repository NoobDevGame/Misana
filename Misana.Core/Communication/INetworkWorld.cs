namespace Misana.Core.Communication
{
    public interface INetworkWorld
    {
        ConnectState ConnectionState { get; }
        void Connect();
        void Disconnect();
    }
}