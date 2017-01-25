using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition(ResponseType = typeof(CreateEntityMessageResponse))]
    public struct CreateEntityMessageRequest
    {
        public int EntityId;
        public int DefinitionId;

        public CreateEntityMessageRequest(int entityId, int definitionId)
        {
            EntityId = entityId;
            DefinitionId = definitionId;
        }
    }

    [MessageDefinition(IsResponse = true)]
    public struct CreateEntityMessageResponse
    {
        public bool Result;

        public CreateEntityMessageResponse(bool result)
        {
            Result = result;
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