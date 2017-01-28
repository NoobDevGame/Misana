namespace Misana.Network
{
    public static class NetworkManager
    {
        public static INetworkClient CreateNetworkClient()
        {
            return new NetworkClient();
        }
    }
}