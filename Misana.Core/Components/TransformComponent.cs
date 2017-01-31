using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;
using Misana.Core.Maps;

namespace Misana.Core.Components
{
    public class TransformComponent : Component<TransformComponent>
    {
        [Copy, Reset]
        public Area CurrentArea;
        [Copy, Reset]
        public Vector2 Position;

        [Copy, Reset(0.5f)]
        public float Radius = 0.5f;

        [Reset]
        public int ParentEntityId;

        public Vector2 Size => new Vector2(2 * Radius, 2 * Radius);
        public Vector2 HalfSize => new Vector2(Radius, Radius);

        [Reset]
        private int CachedParentId;
        [Reset]
        private Entity CachedParent;
        [Reset]
        private TransformComponent CachedParentTransform;

        public Entity Parent(EntityManager manager)
        {
            if (ParentEntityId <= 0)
                return null;

            if (ParentEntityId == CachedParentId)
                return CachedParent;

            CachedParent = manager.GetEntityById(ParentEntityId);
            CachedParentId = ParentEntityId;
            CachedParentTransform = CachedParent.Get<TransformComponent>();
            return CachedParent;
        }

        public Vector2 AbsolutePosition(EntityManager manager)
        {
            if (ParentEntityId == 0)
                return Position;
            
            if (Parent(manager) != null && CachedParentTransform != null)
                return CachedParentTransform.AbsolutePosition(manager) + Position;

            return Position;
        }

        public override void Reset()
        {
            CurrentArea = null;
            Position = Vector2.Zero;
            Radius = 0.5f;

            CachedParentId = 0;
            CachedParent = null;
        }

        public override void CopyTo(TransformComponent other)
        {
            other.CurrentArea = CurrentArea;
            other.Position = Position;
            other.Radius = Radius;
        }
    }
}