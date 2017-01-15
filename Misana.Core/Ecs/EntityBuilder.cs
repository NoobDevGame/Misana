using System;

namespace Misana.Core.Ecs
{
    public class EntityBuilder
    {
        private Component[] _components;
        public EntityBuilder()
        {
            _components = ComponentArrayPool.Take();
        }

        public EntityBuilder Add<T>() where T : Component, new() => Add<T>((Action<T>) null);

        public EntityBuilder Add<T>(Action<T> a) where T : Component, new()
        {
            var c = ComponentRegistry<T>.Take();
            a?.Invoke(c);

            return Add(c);
        }

        public EntityBuilder Add<T>(T component) where T : Component, new()
        {
            if (_components == null)
                throw new InvalidOperationException();

            _components[ComponentRegistry<T>.Index] = component;
            return this;
        }

        public EntityBuilder Remove<T>() where T : Component, new()
        {
            if (_components == null)
                throw new InvalidOperationException();

            _components[ComponentRegistry<T>.Index] = null;
            return this;
        }

        public Entity Commit(EntityManager manager)
        {
            if (_components == null)
                throw new InvalidOperationException();

            var e = new Entity(manager.NextId(), _components, manager);
            manager.AddEntity(e);

            _components = null;

            return e;
        }

        public EntityBuilder CommitAndReturnCopy(EntityManager manager)
        {
            int id;
            return CommitAndReturnCopy(manager, out id);
        }

        public EntityBuilder CommitAndReturnCopy(EntityManager manager, out int id)
        {
            var copy = Copy();
            var entity = Commit(manager);
            id = entity.Id;
            return copy;
        }

        public EntityBuilder Copy()
        {
            var eb = new EntityBuilder();

            for (int i = 0; i < _components.Length; i++)
            {
                if (_components[i] != null)
                {
                    eb._components[i] = ComponentRegistry.Take[i]();
                    _components[i].CopyTo(eb._components[i]);
                }
            }

            return eb;
        }
    }
}