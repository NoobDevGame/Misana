using Misana.Network;
using Misana.Core.Ecs;

namespace Misana.Core.Communication
{
    public abstract class CommunicationMessage<T>: NetworkMessage
        where T : Component
    {

    }
}