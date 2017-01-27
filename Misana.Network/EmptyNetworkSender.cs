namespace Misana.Network
{
    public class EmptyNetworkSender : INetworkSender
    {
        public void SendMessage<T>(ref T message) where T : struct
        {
        }

        public MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct
        {
            return null;
        }

        public void SendResponseMessage<T>(ref T message, byte messageid) where T : struct
        {

        }
    }
}