using System.Threading;

namespace Misana.Core.Network
{
    public static class NetworkManager
    {
        public const int StandardPort = 34560;

        public static int TcpPort { get; private set; } = StandardPort;
        public static int LocalUdpPort { get; private set; } = StandardPort;
        public static int ServerUdpPort { get; private set; } = StandardPort;

        private static int clientId = 0;
        internal static int GetNextId()
        {
            return Interlocked.Increment(ref clientId);
        }

        public static void SetPorts(int tcpPort = StandardPort,
            int localUdpPort = StandardPort,
            int serverUdpPort = StandardPort)
        {
            TcpPort = tcpPort;
            LocalUdpPort = localUdpPort;
            ServerUdpPort = serverUdpPort;
        }
    }
}