using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Misana.Core.Ecs.Changes;
using Misana.Core.Ecs.Meta;
using Misana.Network;

namespace Misana.Core.Ecs
{
    public class EntityManager
    {
        internal static List<Action> OnNewManager = new List<Action>();

        private static int _entityManagerIndex = -1;

        public static int ComponentCount;

        private readonly IntMap<Entity> _entityMap = new IntMap<Entity>(128);
        private readonly EntitiesToRemove _entitiesToRemove = new EntitiesToRemove();
        private readonly EntitiesToAdd _entitiesToAdd = new EntitiesToAdd();
        private readonly EntitesWithChanges _entitesWithChanges;
        private readonly object _nextTickChangeLock = new object();
        private readonly List<EntityChange> _changesNextTick = new List<EntityChange>();
        internal readonly List<BaseSystem> Systems;

        public readonly int Index;
        
        public GameTime GameTime;
        public readonly SimulationMode Mode;
        public string Name { get; set; }

        public static void Initialize()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var concreteTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).ToList();
            
            ComponentInitializer.Initialize(concreteTypes, out ComponentCount, out OnNewManager);
        }

        private EntityManager(List<BaseSystem> systems, SimulationMode mode)
        {
            _entitesWithChanges =  new EntitesWithChanges(this);
            Index = Interlocked.Increment(ref _entityManagerIndex);
            Mode = mode;
            foreach (var fn in OnNewManager)
                fn();

            foreach (var s in systems)
                s.Initialize(this);

            Systems = systems;
        }
        
        public static EntityManager Create(string name, List<BaseSystem> systems, SimulationMode mode)
        {

            var manager =  new EntityManager(systems, mode)
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
            
            Tick();
            ApplyChanges();
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
            ComponentRegistry.Copy[ComponentRegistry<T>.Index](template, a.Component);
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

        public void AddEntitieIds(int start, int count)
        {
            lock (_idLock)
            {
                for (int i = start; i < start + count; i++)
                {
                    availableEntityIds.Enqueue(i);
                }
            }
        }

        public void Clear()
        {
            while (true)
            {
                var ids = _entityMap.Keys;

                if (ids.Count == 0)
                {
                    break;
                }

                foreach (var id in ids)
                    RemoveEntity(id);

                ApplyChanges();
            }
        }

        public Entity RemoveEntity(int id)
        {
            Entity e;

            if (_entityMap.TryGetValue(id, out e))
            {
                RemoveEntity(e);
            }

            return e;
        }

        public void RemoveEntity(Entity e)
        {
            if (_entityMap.Remove(e.Id))
            {
                _entitiesToRemove.Add(e);
            }
        }

        public Entity DetachEntity(int id)
        {
            Entity e;

            if (_entityMap.TryGetValue(id, out e))
            {
                DetachEntity(e);
            }

            return e;
        }

        public void DetachEntity(Entity e)
        {
            if (_entityMap.Remove(e.Id))
            {
                _entitiesToRemove.Add(e, true);
            }
        }

        public Entity GetEntityById(int id)
        {
            Entity e;
            _entityMap.TryGetValue(id, out e);

            return e;
        }

        private readonly object _idLock = new object();

        public int NextId()
        {
            lock (_idLock)
            {
                return availableEntityIds.Dequeue();
            }
        }
        
        private Queue<int> availableEntityIds = new Queue<int>();

        public List<Tuple<byte[], bool>> Messages = new List<Tuple<byte[], bool>>(); 

        private readonly object _messageLock = new object();

        public void NoteForSend<T>(T msg) where T : struct
        {
            var bytes = MessageHandle<T>.Serialize(ref msg);

            lock (_messageLock)
            {
                Messages.Add(Tuple.Create(bytes, MessageHandle<T>.IsUDPMessage));
            }
        }
    }
}