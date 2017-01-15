namespace Misana.Core.Ecs.Changes
{
    public class ComponentRemoval<T> : EntityChange where T : Component, new()
    {
        public ComponentRemoval(int entityId) : base(entityId, ComponentRegistry<T>.Index) { }

        internal override void Reconcile(EntityChange laterChange)
        {
            throw new System.NotSupportedException();
        }

        internal override void ApplyTo(EntityManager manager, Entity e)
        {
            var existing = e.Get<T>();
            if (existing == null)
            {
                return;
            }

            e.Components[Index] = null;

            foreach (var s in ComponentRegistry<T>.InterestedSystems[manager.Index])
                s.EntityChanged(e);

            foreach (var a in ComponentRegistry<T>.RemovalHooks[manager.Index])
                a(manager, e, existing);

            ComponentRegistry<T>.Release(existing);
        }
    }
}