using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Misana.Core.Ecs
{
    internal static class ComponentInitializer
    {
        public static readonly Dictionary<string, Action<Entity, BinaryReader>> Deserializers;
        public static readonly int ComponentCount;

        static ComponentInitializer()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var concreteTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).ToList();

            // Component Setup
            var baseComponentType = typeof(Component);
            var componentTypes = concreteTypes
                .Where(t => baseComponentType.IsAssignableFrom(t))
                .ToList();

            var registryType = typeof(ComponentRegistry<>);

            ComponentCount = componentTypes.Count;
            Deserializers = new Dictionary<string, Action<Entity, BinaryReader>>(ComponentCount);
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

                var deserializeMethod = componentType.GetMethod(
                    "Deserialize",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    new[] {
                        typeof(Entity),
                        componentType,
                        typeof(BinaryReader)
                    },
                    null);
                if (deserializeMethod == null)
                {
                    throw new Exception(
                        $"Component {componentType.FullName} has no valid deserialize method. Expected signature: static void Deserialize(Entity,{componentType.FullName},BinaryReader)");
                }

                var p1 = Expression.Parameter(typeof(Entity));
                var p3 = Expression.Parameter(typeof(BinaryReader));
                var variable = Expression.Variable(componentType);

                var getMethod = rType.GetMethod("Take", BindingFlags.Static | BindingFlags.Public);

                Deserializers[componentType.FullName] = Expression.Lambda<Action<Entity, BinaryReader>>(
                    Expression.Block(
                        new[] {
                            variable
                        },
                        Expression.Assign(variable, Expression.Call(null, getMethod)),
                        Expression.Assign(Expression.ArrayAccess(Expression.Field(p1, typeof(Entity).GetField("Components")), Expression.Constant(i)), variable),
                        Expression.Call(deserializeMethod, p1, variable, p3)
                    ),
                    false,
                    p1,
                    p3).Compile();
            }
        }
    }
}