namespace Misana.Network.Messages
{
    internal struct GetMessageIDMessageRequest
    {
        public readonly string TypeName;

        public GetMessageIDMessageRequest(string typeName)
        {
            TypeName = typeName;
        }
    }

    internal struct GetMessageIDMessageResponse
    {
        public readonly int typeId;

        public GetMessageIDMessageResponse(int id)
        {
            typeId = id;
        }
    }
}