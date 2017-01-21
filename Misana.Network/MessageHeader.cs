namespace Misana.Network
{
    internal struct MessageHeader
    {
        public MessageHeaderState State;
        public int MessageIndex;

        public MessageHeader(MessageHeaderState state,int messageIndex)
        {
            State = state;
            MessageIndex = messageIndex;
        }

        public MessageHeader(MessageInformation information,int messageIndex)
        {
            State = information.State;
            MessageIndex = messageIndex;
        }
    }
}