using System.Collections.Generic;

namespace Misana.Network
{
    public class BroadcastList<T> : List<T> , IBroadcastSender
        where T : INetworkSender , INetworkClient
    {
        public void SendMessage<T1>(ref T1 message) where T1 : struct
        {
            foreach (var item in this)
            {
                item.SendMessage(ref message);
            }
        }

        public void SendMessage<T1>(ref T1 message, int originId) where T1 : struct
        {
            foreach (var item in this)
            {
                if (item.ClientId == originId)
                    continue;

                item.SendMessage(ref message);
            }
        }
    }
}