using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition( ResponseType = typeof(CreateWorldResponseMessage))]
    public struct CreateWorldRequestMessage
    {

    }

    [MessageDefinition(IsResponse = true)]
    public struct CreateWorldResponseMessage
    {

    }
}