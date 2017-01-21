namespace Misana.Network
{
    public interface INetworkClient
    {
        void SendMessage<T>(ref T message)
            where T : struct ;

        bool TryGetMessage<T>(out T? message)
            where T : struct ;
    }
}