using System;

namespace Misana.Core.Ecs.Changes
{
    internal class ManagedComponentAddition<T> : ComponentAddition<T> where T : Component, new()
    {
        public T Template;
        public T Component;

        internal override void Reconcile(EntityChange laterChange)
        {
            var other = laterChange as ManagedComponentAddition<T>;

            if (other == null)
                throw new InvalidOperationException();

            if (Component != null )
            {
                if (other.Component == null)
                {
                    ComponentRegistry<T>.ReleaseManagedAddition(other);
                    return;
                }

                Template = null;
                Component = other.Component;
            }
            else if (other.Component != null)
            {
                Template = null;
                Component = other.Component;
            }
            else if (other.Template != null)
            {
                Template = other.Template;
            }

            ComponentRegistry<T>.ReleaseManagedAddition(other);
        }

        internal override void Release()
        {
            ComponentRegistry<T>.ReleaseManagedAddition(this);
        }

        internal override void ApplyTo(EntityManager manager, Entity e)
        {
            var existing = e.Get<T>();
            if (existing == null)
            {
                if (Component != null)
                    e.Components[Index] = Component;
                else
                {
                    var c = ComponentRegistry<T>.Take();
                    Template?.CopyTo(c);
                    e.Components[Index] = c;
                }

                foreach(var s in ComponentRegistry<T>.InterestedSystems[manager.Index])
                    s.EntityChanged(e);
            }
            else
            {
                if (Component != null)
                    Component.CopyTo(existing);
                else
                {
                    Template?.CopyTo(existing);
                }
            }

            ComponentRegistry<T>.ReleaseManagedAddition(this);
        }
    }
}