using System.Collections.Generic;
using Misana.Core.Ecs;

namespace Misana.Core.Components.Events
{
    public class EntityFlagComponent : Component<EntityFlagComponent>
    {
        private Dictionary<string,bool> flags = new Dictionary<string, bool>();

        public void Set(string name)
        {
            flags[name] = true;
        }

        public void Reset(string name)
        {
            flags[name] = false;
        }

        public bool Get(string name)
        {
            if (flags.ContainsKey(name))
                return flags[name];

            return false;
        }

        public override void CopyTo(EntityFlagComponent other)
        {
            other.flags = new Dictionary<string, bool>(flags);
        }
    }
}