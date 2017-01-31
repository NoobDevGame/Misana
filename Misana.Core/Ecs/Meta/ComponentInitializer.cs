using System;
using System.Collections;
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
            ComponentRegistry.Reset = new Action<Component>[componentCount];
            ComponentRegistry.Copy = new Action<Component, Component>[componentCount];
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
                
                GenerateResetMethod(i, componentType, baseComponentParam);
                GenerateCopyMethod(i, componentType, baseComponentType);
            }
        }

        private static void GenerateCopyMethod(int i, Type componentType, Type baseComponentType)
        {
            var fieldsToCopy = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.CustomAttributes.Any(a => a.AttributeType == typeof(CopyAttribute))).ToList();

            var sourceVar = Expression.Variable(componentType);
            var targetVar = Expression.Variable(componentType);

            var sourceParam = Expression.Parameter(baseComponentType);
            var targetParam = Expression.Parameter(baseComponentType);

            var blockVariables = new List<ParameterExpression> {
                sourceVar,
                targetVar
            };

            var copyBody = new List<Expression> {
                Expression.Assign(sourceVar, Expression.Convert(sourceParam, componentType)),
                Expression.Assign(targetVar, Expression.Convert(targetParam, componentType))
            };

            var listType = typeof(List<>);
            var genericSelect = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));
            //.Select
            foreach (var f in fieldsToCopy)
            {
                if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == listType)
                {
                    var listItemType = f.FieldType.GenericTypeArguments[0];
                    
                    //Expression.
                    copyBody.Add(Expression.Call(Expression.Field(targetVar, f), f.FieldType.GetMethod("Clear")));

                    if (listItemType.IsValueType)
                    {
                        copyBody.Add(
                            Expression.Call(
                                Expression.Field(targetVar, f),
                                f.FieldType.GetMethod("AddRange"),
                                Expression.Field(sourceVar, f))
                        );
                    }
                    else
                    {
                        var copyMethod = listItemType.GetMethod("Copy");
                        
                        if (copyMethod == null)
                            throw new NotSupportedException($"Component {componentType} - {f.Name} - Copy method missing on {listItemType}");

                        var addMethod = f.FieldType.GetMethod("Add");
                        var argParam = Expression.Parameter(listItemType);

                        copyBody.Add(ForEach(Expression.Field(sourceVar, f), argParam, Expression.Call(Expression.Field(targetVar, f), addMethod, argParam)));
                    }


                }
                else
                {
                    copyBody.Add(Expression.Assign(Expression.Field(targetVar, f), Expression.Field(sourceVar, f)));
                }
            }

            ComponentRegistry.Copy[i] = Expression.Lambda<Action<Component, Component>>(Expression.Block(blockVariables, copyBody), sourceParam, targetParam).Compile();
        }


        private static void GenerateResetMethod(int i, Type componentType, ParameterExpression baseComponentParam)
        {
            var fieldsToReset = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.CustomAttributes.Any(a => a.AttributeType == typeof(ResetAttribute))).ToList();
            
            var cVar = Expression.Variable(componentType);

            var resetBody = new List<Expression>();
            resetBody.Add(Expression.Assign(cVar, Expression.Convert(baseComponentParam, componentType)));

            var listType = typeof(List<>);

            foreach (var f in fieldsToReset)
            {
                var resetAttr = f.GetCustomAttributes(typeof(ResetAttribute)).Cast<ResetAttribute>().First();
                if (resetAttr.DefaultValue != null)
                {
                    resetBody.Add(Expression.Assign(Expression.Field(cVar, f), Expression.Constant(resetAttr.DefaultValue, f.FieldType)));
                    continue;
                }

                if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == listType)
                {
                    resetBody.Add(Expression.Call(Expression.Field(cVar, f), f.FieldType.GetMethod("Clear")));
                }
                else
                {
                    resetBody.Add(Expression.Assign(Expression.Field(cVar, f), Expression.Default(f.FieldType)));
                }
            }

            ComponentRegistry.Reset[i] = Expression.Lambda<Action<Component>>(Expression.Block(new[] { cVar }, resetBody), baseComponentParam).Compile();
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

        public static Expression ForEach(Expression collection, ParameterExpression loopVar, Expression loopContent)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);
            
            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, Expression.Constant(true)),
                        Expression.Block(new[] { loopVar },
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
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