using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Misana.Core.Ecs.Meta
{
    internal static class ComponentInitializer
    {
        internal static void Initialize(List<Type> concreteTypes, out int componentCount, out List<Action> onNewManager)
        {
            var baseComponentType = typeof(Component);
            var componentTypes = concreteTypes
                .Where(t => baseComponentType.IsAssignableFrom(t))
                .ToList();

            var registryType = typeof(ComponentRegistry<>);

            componentCount = componentTypes.Count;
            onNewManager = new List<Action>();

            ComponentArrayPool.Initialize(componentCount);
            ComponentRegistry.Release = new Action<Component>[componentCount];
            ComponentRegistry.Take = new Func<Component>[componentCount];
            ComponentRegistry.AdditionHooks = new Action<EntityManager, Entity, Component>[componentCount];
            ComponentRegistry.RemovalHooks = new Action<EntityManager, Entity, Component>[componentCount];

            var baseComponentParam = Expression.Parameter(baseComponentType);
            var emParam = Expression.Parameter(typeof(EntityManager));
            var eParam = Expression.Parameter(typeof(Entity));

            for (var i = 0; i < componentTypes.Count; i++)
            {
                var componentType = componentTypes[i];
                var rType = registryType.MakeGenericType(componentType);

                InitializeRegistry(i, rType, componentType);
                AssignRegistryPoolingMethods(i, rType, componentType, baseComponentParam);
                AssignRegistryHooks(i, rType, componentType, baseComponentParam, emParam, eParam);

                componentType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.Invoke(null, null);
                var onmt = rType.GetMethod("OnNewManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                onNewManager.Add(Expression.Lambda<Action>(Expression.Call(null, onmt), false).Compile());
            }
        }

        private static void AssignRegistryHooks(int i, Type rType, Type componentType, ParameterExpression cParam, ParameterExpression emParam, ParameterExpression eParam)
        {
            var genericAdditionHooks = rType.GetMethod("TriggerAdditionHooks", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            ComponentRegistry.AdditionHooks[i] =
                Expression.Lambda<Action<EntityManager, Entity, Component>>(
                    Expression.Call(null, genericAdditionHooks, emParam, eParam, Expression.Convert(cParam, componentType)), false, emParam, eParam, cParam)
                    .Compile();

            var genericRemovalHooks = rType.GetMethod("TriggerRemovalHooks", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            ComponentRegistry.RemovalHooks[i] =
                Expression.Lambda<Action<EntityManager, Entity, Component>>(
                    Expression.Call(null, genericRemovalHooks, emParam, eParam, Expression.Convert(cParam, componentType)), false, emParam, eParam, cParam)
                    .Compile();
        }

        private static void AssignRegistryPoolingMethods(int componentIndex, Type rType, Type componentType, ParameterExpression cParam)
        {
            var genericRelease = rType.GetMethod("Release", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            ComponentRegistry.Release[componentIndex] =
                Expression.Lambda<Action<Component>>(Expression.Call(null, genericRelease, Expression.Convert(cParam, componentType)), false, cParam)
                    .Compile();

            var genericTake = rType.GetMethod("Take", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            ComponentRegistry.Take[componentIndex] =
                Expression.Lambda<Func<Component>>(Expression.Call(null, genericTake), false)
                    .Compile();
        }

        private static void InitializeRegistry(int index, Type registryType, Type componentType)
        {
            var attr = componentType.GetCustomAttributes(typeof(ComponentConfigAttribute), false);
            
            var prefill = 16;
            if (attr.Length > 0)
            {
                var a = (ComponentConfigAttribute)attr[0];
                prefill = a.Prefill;
            }

            registryType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Invoke(null, new object[] { index, prefill });
        }
    }
}