using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition(ResponseType = typeof(CreateEntityMessageResponse))]
    public struct CreateEntityMessageRequest
    {

        public int DefinitionId;

        public CreateEntityMessageRequest(int definitionId)
        {

            DefinitionId = definitionId;
        }
    }

    [MessageDefinition(IsResponse = true)]
    public struct CreateEntityMessageResponse
    {
        public bool Result;
        public int EntityId;

        public CreateEntityMessageResponse(bool result,int entityId)
        {
            Result = result;
            EntityId = entityId;
        }
    }

    [MessageDefinition()]
    public struct OnCreateEntityMessage
    {
        public int EntityId;
        public int DefinitionId;

        public OnCreateEntityMessage(int entityId, int definitionId)
        {
            EntityId = entityId;
            DefinitionId = definitionId;
        }
    }
}