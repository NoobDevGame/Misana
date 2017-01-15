using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Misana.Core.Ecs.Changes;

namespace Misana.Core.Ecs
{
    // ReSharper disable StaticMemberInGenericType
    public class ComponentRegistry<T> where T : Component, new()
    {
        private static ConcurrentStack<T> _freeList;
        private static List<ManagedComponentAddition<T>> _additionFreeList;
        private static int _additionIndex;
        private static object _listLock = new object();

        private static readonly Mutex M = new Mutex(false);

        public static List<BaseSystem>[] InterestedSystems;

        // ReSharper disable once UnusedMember.Local
        private static void OnNewManager() // Called via reflection
        {
            if (InterestedSystems == null)
            {
                InterestedSystems = new [] { new List<BaseSystem>() };
            }
            else
            {
                M.WaitOne();
                InterestedSystems = new List<List<BaseSystem>>(InterestedSystems) {
                    new List<BaseSystem>()
                }.ToArray();
                M.ReleaseMutex();
            }
        }
        
        public static int Index;

        // ReSharper disable once UnusedMember.Local
        private static void Initialize(int index, int prefill) // Called via reflection
        {
            Index = index;
            _freeList = new ConcurrentStack<T>();

            for (int i = 0; i < prefill; i++)
            {
                _freeList.Push(new T());
            }

            _additionFreeList = new List<ManagedComponentAddition<T>>();

            for (int i = 0; i < prefill; i++)
            {
                _additionFreeList.Add(new ManagedComponentAddition<T>());
            }

            _additionIndex = prefill - 1;
        }

        public static void Release(T item)
        {
            item.Reset();
            _freeList.Push(item);
        }

        public static T Take()
        {
            T item;

            if (!_freeList.TryPop(out item))
                item = new T();

            return item;
        }


        internal static ManagedComponentAddition<T> TakeManagedAddition()
        {
            if (_additionIndex < 0)
                return new ManagedComponentAddition<T>();

            lock (_listLock)
            {
                if (_additionIndex < 0)
                    return new ManagedComponentAddition<T>();

                return _additionFreeList[_additionIndex--];
            }
        }

        internal static void ReleaseManagedAddition(ManagedComponentAddition<T> a)
        {
            a.Component = null;
            a.Template = null;
            a.EntityId = 0;

            lock (_listLock)
            {
                if(++_additionIndex >= _additionFreeList.Count)
                    _additionFreeList.Add(a);
                else
                {
                    _additionFreeList[_additionIndex] = a;
                }
            }
            
        }
    }

    public static class ComponentRegistry
    {
        public static Action<Component>[] Release;
        public static Func<Component>[] Take;
    }
}