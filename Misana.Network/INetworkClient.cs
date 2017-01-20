namespace Misana.Network
{
    public interface INetworkClient
    {
        void SendMessageFast(NetworkMessage message);
    }
}