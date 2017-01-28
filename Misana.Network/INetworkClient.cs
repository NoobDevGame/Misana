using System.Net;
using System.Threading.Tasks;

namespace Misana.Network
{
    public interface INetworkClient : INetworkSender, INetworkReceiver, INetworkIdentifier
    {
        Task Connect(IPEndPoint ipEndPoint);
        void Disconnect();
        bool IsConnected { get; }
    }
}