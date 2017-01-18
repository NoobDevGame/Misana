using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class WieldedComponent : Component<WieldedComponent>
    {
        public Vector2 ParentFacing;
        public Vector2 ParentPosition;
        public float Distance;
        public bool Use;

        public override void CopyTo(WieldedComponent other)
        {
            other.ParentFacing = ParentFacing;
            other.Distance = Distance;
        }
    }
}