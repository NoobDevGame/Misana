using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class WieldedComponent : Component<WieldedComponent>
    {
        // Fixed
        public Vector2 Offset;

        // Config
        public bool Use;

        // Working Set
        public Vector2 ParentFacing;
        public Vector2 ParentPosition;
        
        // Ref (self)
        public SpawnerComponent Spawner;
        
        public override void CopyTo(WieldedComponent other)
        {
            other.ParentFacing = ParentFacing;
            other.Offset = Offset;
            other.ParentPosition = ParentPosition;
        }
    }
}