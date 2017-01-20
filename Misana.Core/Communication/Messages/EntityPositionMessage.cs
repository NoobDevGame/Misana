using Misana.Core.Components;

namespace Misana.Core.Communication.Messages
{
    public class EntityPositionMessage : CommunicationMessage<TransformComponent>
    {
        public override uint MessageTypeId => (uint)MessageType.EntityPosition;
    }
}