﻿using System;
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
        
        internal readonly List<BaseSystem> Systems;

        public readonly int Index;

        private int _entityId;

        public GameTime GameTime;

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

                var genericTake = rType.GetMethod("Take", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                
                ComponentRegistry.Take[i] =
                    Expression.Lambda<Func<Component>>(Expression.Call(null, genericTake), false)
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
            return new EntityManager(systems);
        }

        public void ApplyChanges()
        {
            if(_entitiesToRemove.HasChanges)
                _entitiesToRemove.Commit(Systems);

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

        internal EntityManager Add(EntityChange change)
        {
            _entitesWithChanges.Add(change);
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
            while (_entities.Count > 0)
            {
                RemoveEntity(_entities[0]);
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

        public int NextId() => Interlocked.Increment(ref _entityId);
    }
}