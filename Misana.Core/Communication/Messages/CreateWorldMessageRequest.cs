using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition( ResponseType = typeof(CreateWorldMessageResponse))]
    public struct CreateWorldMessageRequest
    {

    }

    [MessageDefinition(IsResponse = true)]
    public struct CreateWorldMessageResponse
    {
        public bool Result;
        public int EntityStartIndex;
        public int EntityCount;

        public CreateWorldMessageResponse(bool result,int entityStartIndex,int entityCount)
        {
            Result = result;
            EntityStartIndex = entityStartIndex;
            EntityCount = entityCount;
        }
    }
}