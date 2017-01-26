using System;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkEffectMessenger
    {
        private readonly INetworkSender _sender;
        private readonly INetworkReceiver _receiver;

        public NetworkEffectMessenger(INetworkSender sender,INetworkReceiver receiver)
        {
            _sender = sender;
            _receiver = receiver;
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            _sender.SendMessage(ref message);
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            return _receiver.TryGetMessage(out message);
        }
    }
}