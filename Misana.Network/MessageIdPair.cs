using System;
using System.Threading;

namespace Misana.Network
{
    public class MessageIdPair
    {
        public readonly int SystemId;
        public readonly Type MessageType;

        public MessageIdPair(int systemId, Type messageType)
        {
            SystemId = systemId;
            MessageType = messageType;
        }
    }

    public sealed class MessageIdPair<T> : MessageIdPair
        where T : struct
    {
        public MessageIdPair(int systemId)
            : base(systemId, typeof(T))
        {
        }
    }
}