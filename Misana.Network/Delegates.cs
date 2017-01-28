namespace Misana.Network
{
    public delegate void MessageReceiveCallback<in T>(T message, MessageHeader header,INetworkClient client)
        where T: struct ;
}