using System;

namespace Misana.Network
{
    public interface INetworkClient
    {

        void RegisterOnMessageCallback<T>(Action<T> callback)
            where T : struct ;

        void SendMessage<T>(ref T message)
            where T : struct ;

        bool TryGetMessage<T>(out T? message)
            where T : struct ;

        void Connect();
        void Disconnect();
        bool IsConnected { get; }
    }
}