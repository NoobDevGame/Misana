namespace Misana.Network
{
    public delegate void MessageReceiveCallback<in T>(T message, MessageHeader header,NetworkClient client)
        where T: struct ;
}