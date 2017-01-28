using System;

namespace Misana.Network
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class MessageDefinitionAttribute : Attribute
    {
        public Type ResponseType { get; set; }
        public bool IsResponse { get; set; }
        public bool UseUDP { get; set; }
    }
}