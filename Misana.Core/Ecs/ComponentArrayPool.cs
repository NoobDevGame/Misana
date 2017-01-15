using System.Collections.Concurrent;
using Misana.Core.Ecs.Changes;

namespace Misana.Core.Ecs
{
    internal static class ComponentArrayPool
    {
        private static ConcurrentStack<Component[]> _freeList;
        
        public static void Initialize(int componentCount)
        {
            const int prefill = 2000;

            _freeList = new ConcurrentStack<Component[]>();

            for (int i = 0; i < prefill; i++)
            {
                _freeList.Push(new Component[componentCount]);
            }
        }

        public static Component[] Take()
        {
            Component[] item;
            if (!_freeList.TryPop(out item))
                item = new Component[EntityManager.ComponentCount];

            return item;
        }

        public static void Release(Component[] arr)
        {
            for (int i = 0; i < EntityManager.ComponentCount; i++)
            {
                if (arr[i] != null && !arr[i].Unmanaged)
                {
                    ComponentRegistry.Release[i](arr[i]);
                }

                arr[i] = null;
            }
            _freeList.Push(arr);
        }
    }
}