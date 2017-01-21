using System;
using System.Threading;

namespace Misana.Network
{
    public abstract class MessageIdPair
    {
        public readonly int SystemId;
        public readonly Type MessageType;

        protected MessageIdPair(int systemId, Type messageType)
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