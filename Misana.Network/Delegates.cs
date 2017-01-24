namespace Misana.Network
{
    public delegate void MessageReceiveCallback<in T>(T message, MessageHeader header)
        where T: struct ;
}