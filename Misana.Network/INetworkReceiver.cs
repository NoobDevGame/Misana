using System.Collections.Generic;

namespace Misana.Network
{
    public interface INetworkReceiver
    {
        bool TryGetMessage<T>(out T message, out INetworkIdentifier senderClient)
            where T: struct ;

        bool TryGetMessage<T>(out T message)
            where T: struct ;

        void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback)
            where T : struct;
    }
}