using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Misana.Core.Ecs.Changes;

namespace Misana.Core.Ecs
{
    public class EntityManager
    {
        internal static readonly List<Action> OnNewManager = new List<Action>();

        private static int _entityManagerIndex = -1;

        public static readonly int ComponentCount;

        private readonly Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
        private readonly EntitiesToRemove _entitiesToRemove = new EntitiesToRemove();
        private readonly EntitiesToAdd _entitiesToAdd = new EntitiesToAdd();
        private readonly EntitesWithChanges _entitesWithChanges;
        private readonly object _nextTickChangeLock = new object();
        private readonly List<EntityChange> _changesNextTick = new List<EntityChange>();
        internal readonly List<BaseSystem> Systems;

        public readonly int Index;

        private int _entityId;
        private int _maxEntityId = short.MaxValue;

        public GameTime GameTime;

        public string Name { get; set; }

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
            ComponentRegistry.Take = new Func<Component>[ComponentCount];
            ComponentRegistry.AdditionHooks = new Action<EntityManager, Entity, Component>[ComponentCount];
            ComponentRegistry.RemovalHooks = new Action<EntityManager, Entity, Component>[ComponentCount];

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

                var cParam = Expression.Parameter(baseComponentType);

                var genericRelease = rType.GetMethod("Release", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                ComponentRegistry.Release[i] =
                    Expression.Lambda<Action<Component>>(Expression.Call(null, genericRelease, Expression.Convert(cParam, componentType)), false, cParam)
                        .Compile();

                var genericTake = rType.GetMethod("Take", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                ComponentRegistry.Take[i] =
                    Expression.Lambda<Func<Component>>(Expression.Call(null, genericTake), false)
                        .Compile();

                var emParam = Expression.Parameter(typeof(EntityManager));
                var eParam = Expression.Parameter(typeof(Entity));

                var genericAdditionHooks = rType.GetMethod("TriggerAdditionHooks", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                ComponentRegistry.AdditionHooks[i] =
                    Expression.Lambda<Action<EntityManager, Entity, Component>>(
                        Expression.Call(null, genericAdditionHooks, emParam, eParam,Expression.Convert(cParam,componentType)), false, emParam, eParam, cParam)
                        .Compile();

                var genericRemovalHooks = rType.GetMethod("TriggerRemovalHooks", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                ComponentRegistry.RemovalHooks[i] =
                    Expression.Lambda<Action<EntityManager, Entity, Component>>(
                        Expression.Call(null, genericRemovalHooks, emParam, eParam, Expression.Convert(cParam, componentType)), false, emParam, eParam, cParam)
                        .Compile();

                componentType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.Invoke(null, null);
                var onmt = rType.GetMethod("OnNewManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                OnNewManager.Add(Expression.Lambda<Action>(Expression.Call(null, onmt), false).Compile());
            }
        }

        private EntityManager(List<BaseSystem> systems)
        {
            _entitesWithChanges =  new EntitesWithChanges(this);
            Index = Interlocked.Increment(ref _entityManagerIndex);

            foreach (var fn in OnNewManager)
                fn();

            foreach (var s in systems)
                s.Initialize(this);

            Systems = systems;
        }
        
        public static EntityManager Create(string name, List<BaseSystem> systems)
        {

            var manager =  new EntityManager(systems)
            {
                Name = name,
            };

            return manager;
        }

        public EntityManager RegisterAdditionHook<T>(Action<EntityManager, Entity, T> h) where T : Component, new()
        {
            ComponentRegistry<T>.AdditionHooks[Index].Add(h);
            return this;
        }

        public EntityManager RegisterRemovalHook<T>(Action<EntityManager, Entity, T> h) where T : Component, new()
        {
            ComponentRegistry<T>.RemovalHooks[Index].Add(h);
            return this;
        }

        public void ApplyChanges()
        {
            if(_entitiesToRemove.HasChanges)
                _entitiesToRemove.Commit(Systems);

            if (_changesNextTick.Count > 0)
            {
                lock (_nextTickChangeLock)
                {
                    foreach (var c in _changesNextTick)
                        _entitesWithChanges.Add(c);

                    _changesNextTick.Clear();
                }
            }

            if(_entitesWithChanges.HasChanges)
                _entitesWithChanges.Commit();

            if(_entitiesToAdd.HasChanges)
                _entitiesToAdd.Commit(Systems);
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

        public EntityManager Add<T>(Entity e) where T : Component, new()
        {
            var a = ComponentRegistry<T>.TakeManagedAddition();
            a.EntityId = e.Id;
            _entitesWithChanges.Add(a);
            return this;
        }

        internal EntityManager Change(EntityChange change)
        {
            _entitesWithChanges.Add(change);
            return this;
        }

        public EntityManager ChangeNextTick(EntityChange change)
        {
            lock (_nextTickChangeLock)
            {
                _changesNextTick.Add(change);
            }

            return this;
        }

        public EntityManager Add<T>(Entity e, T component, bool expectedUnmanaged = true) where T : Component, new()
        {
            if (component.Unmanaged)
            {
                _entitesWithChanges.Add(new UnmanagedComponentAddition<T>(e.Id, component));
            }
            else
            {
                if(expectedUnmanaged)
                    throw new InvalidOperationException();
                var a = ComponentRegistry<T>.TakeManagedAddition();
                a.EntityId = e.Id;
                a.Component = component;
                _entitesWithChanges.Add(a);
            }
            
            return this;
        }

        public EntityManager AddViaTemplate<T>(Entity e, T template) where T : Component, new()
        {
            var a = ComponentRegistry<T>.TakeManagedAddition();
            a.EntityId = e.Id;
            a.Component = ComponentRegistry<T>.Take();
            template.CopyTo(a.Component);
            _entitesWithChanges.Add(a);
            return this;
        }

        public EntityManager Remove<T>(Entity e) where T : Component, new()
        {
            _entitesWithChanges.Add(new ComponentRemoval<T>(e.Id));
            return this;
        }

        public Entity AddEntity(Entity e)
        {
            if (e.Manager != this)
                throw new ArgumentException("Entity Manager mismatch on Entity addition", nameof(e));
            
            _entityMap[e.Id] = e;
            _entitiesToAdd.Add(e);

            return e;
        }

        public void Clear()
        {
            while (true)
            {
                var ids = _entityMap.Keys.ToList();

                if (ids.Count == 0)
                {
                    break;
                }

                Entity e;

                foreach (var id in ids)
                    RemoveEntity(id);

                ApplyChanges();
            }

            _entityId = 0;
        }

        public Entity RemoveEntity(int id)
        {
            Entity e;

            if (_entityMap.TryGetValue(id, out e))
            {
                _entityMap.Remove(id);
                _entitiesToRemove.Add(e);
            }

            return e;
        }

        public Entity GetEntityById(int id)
        {
            Entity e;
            _entityMap.TryGetValue(id, out e);

            return e;
        }

        public int NextId()
        {
            var id = Interlocked.Increment(ref _entityId);
            if (id > _maxEntityId)
                throw new IndexOutOfRangeException("Entity out of Range");
            return id;
        }
    }
}