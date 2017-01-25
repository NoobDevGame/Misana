namespace Misana.Network
{
    public interface INetworkSender
    {
        void SendMessage<T>(ref T message) where T : struct;
    }
}