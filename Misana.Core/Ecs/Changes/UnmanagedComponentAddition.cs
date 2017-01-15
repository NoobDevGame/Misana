using System;

namespace Misana.Core.Ecs.Changes
{
    public class UnmanagedComponentAddition<T> : ComponentAddition<T> where T : Component, new()
    {
        public UnmanagedComponentAddition(int entityId, T component) : base(entityId)
        {
            ComponentToAdd = component;
        }

        public readonly T ComponentToAdd;

        internal override void Reconcile(EntityChange laterChange)
        {
            if(laterChange is UnmanagedComponentAddition<T>)
                throw new InvalidOperationException();
        }

        internal override void ApplyTo(EntityManager manager, Entity e)
        {
            var existing = e.Get<T>();
            if (existing != null)
            {
                throw new InvalidOperationException();
            }

            e.Components[Index] = ComponentToAdd;

            foreach (var a in ComponentRegistry<T>.AdditionHooks[manager.Index])
                a(manager, e, ComponentToAdd);

            foreach (var s in ComponentRegistry<T>.InterestedSystems[manager.Index])
                s.EntityChanged(e);
        }
    }
}