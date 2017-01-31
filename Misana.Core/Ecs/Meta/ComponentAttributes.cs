using System;

namespace Misana.Core.Ecs.Meta
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CopyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ResetAttribute : Attribute
    {
        public readonly object DefaultValue;
        public ResetAttribute(object defaultValue = null)
        {
            DefaultValue = defaultValue;
        }
    }
}