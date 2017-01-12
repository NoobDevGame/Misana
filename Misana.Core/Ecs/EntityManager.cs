using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Misana.Core.Ecs
{
    public class EntityManager
    {
        internal static readonly List<Action> OnNewManager = new List<Action>();

        private static int _entityManagerIndex = -1;

        public IEnumerable<Entity> Entities => _entities;
        private readonly Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
        
        private readonly List<Entity> _entities = new List<Entity>();
        
        private readonly List<Entity> _removedEntities = new List<Entity>();
        public readonly int Index;
        internal readonly List<BaseSystem> Systems;

        private EntityManager(List<BaseSystem> systems)
        {
            Index = Interlocked.Increment(ref _entityManagerIndex);

            foreach (var fn in OnNewManager)
                fn();

            foreach(var s in systems)
                s.Initialize(this);

            Systems = systems;
        }

        public static EntityManager Create(string name, List<BaseSystem> systems)
        {
            return new EntityManager(systems);
        }

        public static readonly int ComponentCount;

        static EntityManager()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var concreteTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).ToList();

            var baseComponentType = typeof(Component);
            var componentTypes = concreteTypes
                .Where(t => baseComponentType.IsAssignableFrom(t))
                .ToList();

            var registryType = typeof(ComponentRegistry<>);

            ComponentCount = componentTypes.Count;
            ComponentArrayPool.Initialize(ComponentCount);
            ComponentRegistry.Release = new Action<Component>[ComponentCount];

            for (var i = 0; i < componentTypes.Count; i++)
            {
                var componentType = componentTypes[i];
                var rType = registryType.MakeGenericType(componentType);

                var attr = componentType.GetCustomAttributes(typeof(ComponentConfigAttribute), false);

                var prefill = 16;
                if (attr.Length > 0)
                {
                    var a = (ComponentConfigAttribute) attr[0];
                    prefill = a.Prefill;
                }

                rType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(
                    null,
                    new object[] {
                        i,
                        prefill
                    });

                var genericRelease = rType.GetMethod("Release", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                var cParam = Expression.Parameter(baseComponentType);
                ComponentRegistry.Release[i] =
                    Expression.Lambda<Action<Component>>(Expression.Call(null, genericRelease, Expression.Convert(cParam, componentType)), false, cParam)
                        .Compile();

                componentType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.Invoke(null, null);
                var onmt = rType.GetMethod("OnNewManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                EntityManager.OnNewManager.Add(Expression.Lambda<Action>(Expression.Call(null, onmt), false).Compile());
            }
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

        public EntityManager Add<T>(Entity e, T component, bool expectedUnmanaged = true) where T : Component, new()
        {
            if(expectedUnmanaged && !component.Unmanaged)
                throw new InvalidOperationException();

            var idx = ComponentRegistry<T>.Index;
            e.Components[idx] = component;

            if (e.Complete)
            {
                foreach (var s in ComponentRegistry<T>.InterestedSystems[Index])
                    s.EntityChanged(e);
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
            else
            {
                action(existing);
            }

            return this;
        }

        public EntityManager Remove<T>(Entity e) where T : Component, new()
        {
            var cmp = e.Get<T>();

            if (cmp == null)
                return this;

            if(!cmp.Unmanaged)
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

            _entities.Add(e);
            _entityMap[e.Id] = e;

            e.Complete = true;

            foreach (var s in Systems)
                s.EntityAdded(e);

            return e;
        }

        public Entity RemoveEntity(int id)
        {
            Entity e;

            if (_entityMap.TryGetValue(id, out e))
            {
                _entityMap.Remove(id);
                RemoveEntity(e);
            }

            return e;
        }

        public Entity GetEntityById(int id)
        {
            Entity e;
            _entityMap.TryGetValue(id, out e);

            return e;
        }

        public void RemoveEntity(Entity entity)
        {
            _entityMap.Remove(entity.Id);
            _removedEntities.Add(entity);
        }
    }
}