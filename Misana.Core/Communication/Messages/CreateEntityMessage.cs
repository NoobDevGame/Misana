using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition]
    public struct CreateEntityMessage
    {
        public int EntityId;
        public int DefinitionId;

        public CreateEntityMessage(int entityId, int definitionId)
        {
            EntityId = entityId;
            DefinitionId = definitionId;
        }
    }
}