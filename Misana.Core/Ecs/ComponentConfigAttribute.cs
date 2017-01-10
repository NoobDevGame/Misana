using System;

namespace Misana.Core.Ecs
{
    public class ComponentConfigAttribute : Attribute
    {
        public int Prefill;

        public ComponentConfigAttribute(int prefill = 16)
        {
            Prefill = prefill;
        }
    }
}