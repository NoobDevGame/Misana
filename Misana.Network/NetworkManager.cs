using System.Threading;

namespace Misana.Network
{
    public static class NetworkManager
    {
        private static int clientId = 0;

        internal static int GetNextId()
        {
            return Interlocked.Increment(ref clientId);
        }

        public static INetworkClient CreateNetworkClient()
        {
            return new NetworkClient();
        }
    }
}