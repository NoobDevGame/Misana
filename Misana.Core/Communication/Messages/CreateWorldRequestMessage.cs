using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition( ResponseType = typeof(CreateWorldResponeMessage))]
    public struct CreateWorldRequestMessage
    {

    }

    [MessageDefinition(IsRespone = true)]
    public struct CreateWorldResponeMessage
    {

    }
}