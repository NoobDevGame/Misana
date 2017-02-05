using System.Collections.Generic;
using Misana.Core.Components;

namespace Misana.Core.Ecs.Changes
{
    internal class EntitiesToAdd
    {
        public readonly List<Entity> List = new List<Entity>(16);

        public void Add(Entity e)
        {
            lock (lockObject)
            {
                List.Add(e);
                HasChanges = true;
            }

        }

        public bool HasChanges;
        private object lockObject = new object();

        public void Commit(List<BaseSystem> systems)
        {
            lock (lockObject)
            {
                foreach (var e in List)
                {
                    for (int i = 0; i < e.Components.Length; i++)
                    {
                        if (e.Components[i] != null)
                        {
                            ComponentRegistry.AdditionHooks[i](e.Manager, e, e.Components[i]);
                        }
                    }

                    foreach (var s in systems)
                        s.EntityAdded(e);
                }

                List.Clear();
                HasChanges = false;
            }
        }
    }
}