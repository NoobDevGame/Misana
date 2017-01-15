namespace Misana.Core.Ecs.Changes
{
    public abstract class EntityChange
    {
        protected EntityChange(int entityId, int componentIndex)
        {
            EntityId = entityId;
            Index = componentIndex;
        }

        protected EntityChange(int componentIndex)
        {
            Index = componentIndex;
        }

        public int EntityId;
        public readonly int Index;
        public bool Addition;

        internal abstract void Reconcile(EntityChange laterChange);
        internal abstract void ApplyTo(EntityManager manager, Entity e);

        internal virtual void Release() { }
    }
}