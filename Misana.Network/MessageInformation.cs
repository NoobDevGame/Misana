namespace Misana.Network
{
    public struct MessageInformation
    {
        public MessageHeaderState State;

        public MessageInformation(MessageHeaderState state)
        {
            State = state;
        }
    }
}