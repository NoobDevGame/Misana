using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Misana.Core.Ecs
{
    public class EntityManager
    {
        internal static readonly List<Action> OnNewManager = new List<Action>();

        private static int _entityManagerIndex = -1;

        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<Entity> _entities = new List<Entity>();
        
        private readonly List<Entity> _removedEntities = new List<Entity>();
        public readonly int Index;
        internal readonly List<BaseSystem> Systems;

        private EntityManager(List<Tuple<Func<EntityManager, BaseSystem>, SystemConfigurationAttribute>> systemConstructorsWithConfig)
        {
            Index = Interlocked.Increment(ref _entityManagerIndex);
            foreach (var fn in OnNewManager)
                fn();
            Systems = systemConstructorsWithConfig.Select(t => t.Item1(this)).ToList();
        }

        public static EntityManager Create(string name, List<Assembly> systemAssemblies)
        {
            return new EntityManager(SystemInitializer.Initialize(systemAssemblies));
        }

        public GameTime GameTime;
        public void ApplyChanges()
        {
            foreach (var e in _removedEntities)
            {
                foreach (var s in Systems)
                    s.EntityRemoved(e);

                ComponentArrayPool.Release(e.Components);

                e.Components = null;
                _entities.Remove(e);
            }
        }

        public void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            ApplyChanges();
            Tick();
        }

        public void Tick()
        {
            foreach (var s in Systems)
            {
                s.Tick();
            }
        }

        public EntityManager Add<T>(Entity e, bool throwOnExists = false) where T : Component, new()
        {
            var idx = ComponentRegistry<T>.Index;
            if (e.Components[idx] == null)
            {
                e.Components[idx] = ComponentRegistry<T>.Take();

                if (e.Complete)
                {
                    foreach (var s in ComponentRegistry<T>.InterestedSystems[Index])
                        s.EntityChanged(e);
                }
            }
            else if (throwOnExists)
            {
                throw new InvalidOperationException($"{typeof(T)} exists on entity");
            }
            return this;
        }

        public EntityManager Add<T>(Entity e, Action<T> action, bool throwOnExists = true) where T : Component, new()
        {
            if (action == null)
                return Add<T>(e);

            var idx = ComponentRegistry<T>.Index;

            var existing = e.Get<T>();

            if (existing == null)
            {
                var item = ComponentRegistry<T>.Take();
                action(item);
                e.Components[idx] = item;

                if (e.Complete)
                {
                    foreach (var s in ComponentRegistry<T>.InterestedSystems[Index])
                        s.EntityAdded(e);
                }
            }
            else if (throwOnExists)
            {
                throw new InvalidOperationException($"{typeof(T)} exists on entity");
            }

            return this;
        }

        public EntityManager Remove<T>(Entity e) where T : Component, new()
        {
            var cmp = e.Get<T>();

            if (cmp == null)
                return this;

            ComponentRegistry<T>.Release(cmp);
            e.Components[ComponentRegistry<T>.Index] = null;

            if (e.Complete)
            {
                foreach (var s in ComponentRegistry<T>.InterestedSystems[Index])
                    s.EntityChanged(e);
            }

            return this;
        }

        public Entity NewEntity()
        {
            var e = new Entity(Interlocked.Increment(ref _entityId)) {
                Components = ComponentArrayPool.Take(),
                Manager = this
            };

            return e;
        }

        private int _entityId = 0;

        public Entity Add(Entity e)
        {
            if(e.Manager != this)
                throw new ArgumentException("Entity Manager mismatch on Entity addition", nameof(e));

            e.Complete = true;

            foreach (var s in Systems)
                s.EntityAdded(e);

            return e;
        }

        public void RemoveEntity(Entity entity)
        {
            _removedEntities.Add(entity);
        }
    }
}