﻿namespace Misana.Network
{
    public interface INetworkSender
    {
        void SendMessage<T>(ref T message) where T : struct;
        void SendTcpBytes(byte[] bytes);
        void SendUdpBytes(byte[] bytes);
        MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct;
        void SendResponseMessage<T>(ref T message, byte messageid) where T : struct;
    }
}