namespace Misana.Network
{
    public class EmptyNetworkReceive : INetworkReceiver
    {
        public bool TryGetMessage<T>(out T message, out INetworkIdentifier senderClient) where T : struct
        {
            senderClient = null;
            message = default(T);
            return false;
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            message = default(T);
            return false;
        }

        public void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback) where T : struct
        {
        }
    }
}