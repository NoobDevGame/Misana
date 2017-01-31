using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class WieldedComponent : Component<WieldedComponent>
    {
        [Copy, Reset]
        public Vector2 Offset;

        [Copy, Reset]
        public bool Use;

        [Copy, Reset]
        public Vector2 ParentFacing;

        [Copy, Reset]
        public Vector2 ParentPosition;
        
        [Reset]
        public SpawnerComponent Spawner;
        
        public override void CopyTo(WieldedComponent other)
        {
            other.ParentFacing = ParentFacing;
            other.Offset = Offset;
            other.ParentPosition = ParentPosition;
        }
    }
}