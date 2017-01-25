using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition(ResponseType = typeof(JoinWorldMessageResponse))]
    public struct JoinWorldRequest
    {

    }

    [MessageDefinition(IsResponse = true)]
    public struct JoinWorldMessageResponse
    {

    }

    [MessageDefinition]
    public struct OnJoinWorldMessage
    {

    }
}