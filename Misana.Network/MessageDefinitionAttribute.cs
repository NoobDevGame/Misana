using System;

namespace Misana.Network
{
    [AttributeUsage(AttributeTargets.Struct,AllowMultiple = false)]
    public class MessageDefinitionAttribute : Attribute
    {
        public MessageDefinitionAttribute(ushort communictaionId)
        {
            if (communictaionId <= byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(communictaionId),communictaionId,$"{nameof(CommunictaionId)} must be greater then {byte.MaxValue}");

            CommunictaionId = communictaionId;
        }

        public ushort CommunictaionId { get; private set; }
    }
}