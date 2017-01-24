namespace Misana.Network
{
    public struct MessageHeader
    {
        public MessageHeaderState State;
        public int MessageTypeIndex;

        public byte MessageId;

        public MessageHeader(int messageTypeIndex,MessageHeaderState state)
        {
            State = state;
            MessageTypeIndex = messageTypeIndex;
            MessageId = 0;
        }

        public MessageHeader(int messageTypeIndex,byte messageId)
        {
            MessageTypeIndex = messageTypeIndex;
            State = 0;
            MessageId = messageId;
        }

        public MessageHeader(int messageTypeIndex,byte messageId,MessageHeaderState state)
        {
            State = state;
            MessageTypeIndex = messageTypeIndex;
            MessageId = messageId;
        }


    }
}