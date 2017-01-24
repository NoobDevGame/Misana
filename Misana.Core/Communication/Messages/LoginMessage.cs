using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition(ResponseType = typeof(LoginResponeMessage))]
    public struct LoginRequestMessage
    {

    }

    [MessageDefinition(IsRespone = true)]
    public struct LoginResponeMessage
    {

    }
}