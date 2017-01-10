using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Misana.Core.Ecs
{
    internal class SystemInitializer
    {
        private static readonly Dictionary<Type, Func<EntityManager, BaseSystem>> SystemConstructors
            = new Dictionary<Type, Func<EntityManager, BaseSystem>>();

        static SystemInitializer()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var concreteTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).ToList();

            var baseSystemType = typeof(BaseSystem);

            var systemTypes = concreteTypes
                .Where(t => baseSystemType.IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .ToList();

            var arg = Expression.Parameter(typeof(EntityManager));

            foreach (var t in systemTypes)
            {
                var constructor = t.GetConstructor(new[] { typeof(EntityManager) });
                if (constructor != null)
                {
                    SystemConstructors[t] = Expression.Lambda<Func<EntityManager, BaseSystem>>(Expression.New(constructor, arg), false, arg).Compile();
                }
            }
        }

        internal static List<Tuple<Func<EntityManager, BaseSystem>, SystemConfigurationAttribute>> Initialize(List<Assembly> systemAssemblies )
        {
            var concreteTypes = systemAssemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).ToList();

            var baseSystemType = typeof(BaseSystem);

            var systemTypes = concreteTypes
                .Where(t => baseSystemType.IsAssignableFrom(t))
                .ToList();

            var validSystems = new List<Tuple<Type, SystemConfigurationAttribute>>();

            foreach (var st in systemTypes)
            {
                var config = st.GetCustomAttributes(typeof(SystemConfigurationAttribute)).FirstOrDefault();
                if (config == null)
                    continue;

                validSystems.Add(Tuple.Create(st, (SystemConfigurationAttribute)config));
            }

            var systemMap = new Dictionary<string, Tuple<Type, SystemConfigurationAttribute>>();

            foreach (var vs in validSystems)
            {
                Action<string[]> validateNames = arr => {
                    if (arr == null || arr.Length == 0)
                        return;

                    foreach (var item in arr)
                    {
                        if (validSystems.Any(s => s.Item1.Name == item))
                            break;

                        throw new ArgumentException($"System named `{item}` not found (Referenced by {vs.Item1.FullName})");
                    }
                };

                validateNames(vs.Item2.After);
                validateNames(vs.Item2.Before);
                validateNames(vs.Item2.ConcurrentlyWith);
                validateNames(vs.Item2.Replaces);
            }

            foreach (var vs in validSystems)
            {
                var replacement = vs.Item2.Replaces;
                if (replacement == null || replacement.Length == 0)
                    continue;

                foreach (var item in replacement)
                    systemMap[item] = vs;
            }

            for (var i = 0; i < validSystems.Count; i++)
            {
                if (systemMap.ContainsKey(validSystems[i].Item1.Name))
                {
                    validSystems.RemoveAt(i--);
                }
                else
                {
                    systemMap[validSystems[i].Item1.Name] = validSystems[i];
                }
            }

            var systems = validSystems.Where(i =>
                (i.Item2.After == null || i.Item2.After.Length == 0)
                && (i.Item2.Before == null || i.Item2.Before.Length == 0)
            ).ToList();

            // System Setup - Ghetto dependency resolution
            for (var j = 0; j < 5; j++)
            {
                for (var i = 0; i < validSystems.Count; i++)
                {
                    var item = validSystems[i];
                    if (systems.Contains(item))
                        continue;

                    Func<string[], bool> allPresent = arr => { return arr.All(s => systems.Contains(systemMap[s])); };

                    var hasDependencies = item.Item2.After != null && item.Item2.After.Length > 0;
                    var affects = item.Item2.Before != null && item.Item2.Before.Length > 0;

                    if (hasDependencies && affects)
                    {
                        if (allPresent(item.Item2.After) && allPresent(item.Item2.Before))
                        {
                            var latest = item.Item2.After.Select(n => systems.IndexOf(systemMap[n])).Max();
                            var least = item.Item2.Before.Select(n => systems.IndexOf(systemMap[n])).Min();

                            int index;

                            if (least > latest)
                                index = least;
                            else if (least == latest)
                            {
                                index = least - 1;
                            }
                            else
                            {
                                continue;
                            }

                            if (index == systems.Count - 1)
                                systems.Add(item);
                            else
                                systems.Insert(index, item);
                        }
                    }
                    else if (hasDependencies)
                    {
                        if (allPresent(item.Item2.After))
                        {
                            var index = item.Item2.After.Select(n => systems.IndexOf(systemMap[n])).Max();
                            if (index == systems.Count - 1)
                                systems.Add(item);
                            else
                                systems.Insert(index + 1, item);
                            validSystems.RemoveAt(i--);
                        }
                    }
                    else if (affects)
                    {
                        if (allPresent(item.Item2.Before))
                        {
                            var index = item.Item2.Before.Select(n => systems.IndexOf(systemMap[n])).Min();
                            systems.Insert(index, item);
                            validSystems.RemoveAt(i--);
                        }
                    }
                }
            }

            return systems.Select(t => Tuple.Create(SystemConstructors[t.Item1], t.Item2)).ToList();
        }
    }
}