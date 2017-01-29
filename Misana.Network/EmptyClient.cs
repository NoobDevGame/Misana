using System.Net;
using System.Threading.Tasks;

namespace Misana.Network
{
    public class EmptyClient : INetworkClient
    {
        public int NetworkId { get; } = NetworkManager.GetNextId();

        public bool IsConnected { get; } = false;
        public bool CanSend { get; } = true;

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

        public bool TryGetMessage<T>(out T message, out INetworkIdentifier senderClient) where T : struct
        {
            message = default(T);
            senderClient = null;
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

        public async Task Connect(IPAddress addr)
        {

        }

        public void Disconnect()
        {
        }
    }
}