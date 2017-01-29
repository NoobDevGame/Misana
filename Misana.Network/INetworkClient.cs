using System.Net;
using System.Threading.Tasks;

namespace Misana.Network
{
    public interface INetworkClient : INetworkSender, INetworkReceiver, INetworkIdentifier
    {
        bool IsConnected { get; }
        bool CanSend { get;}

        Task Connect(IPAddress addr);
        void Disconnect();

    }
}