using System;
using System.Threading;

namespace Misana.Network
{
    public abstract class MessageIdPair
    {
        public readonly int SystemId;
        public readonly ushort CommunictaionId;
        public readonly Type MessageType;

        protected MessageIdPair(int systemId, ushort communictaionId, Type messageType)
        {
            SystemId = systemId;
            CommunictaionId = communictaionId;
            MessageType = messageType;
        }
    }

    public sealed class MessageIdPair<T> : MessageIdPair
        where T : struct
    {
        public MessageIdPair(int systemId, ushort communictaionId)
            : base(systemId, communictaionId, typeof(T))
        {
        }
    }
}