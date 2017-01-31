using System.IO;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Ecs
{
    public enum RunsOn : byte
    {
        Both = 0,
        Server = 1,
        Client = 2
    }

    public abstract class Component
    {
        [Copy, Reset]
        public RunsOn RunsOn;
        public bool Unmanaged;
        public abstract void CopyTo(Component other);
    }

    public abstract class Component<T> : Component where T : Component<T>
    {
        public abstract void CopyTo(T other);
        public override void CopyTo(Component other)
        {
            other.RunsOn = RunsOn;
            CopyTo((T)other);
        }
    }
}