namespace Misana.Network
{
    public interface IBroadcastSender : INetworkSender
    {
        void SendMessage<T>(ref T message,int originId) where T : struct;
    }
}