namespace Misana.Network
{
    public struct MessageHeader
    {
        public MessageHeaderState State;

        public MessageHeader(MessageHeaderState state)
        {
            State = state;
        }
    }
}