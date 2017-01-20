using System.Collections.Generic;
using Misana.Core.Components;

namespace Misana.Core.Ecs.Changes
{
    internal class EntitiesToAdd
    {
        private readonly List<Entity> _list = new List<Entity>(16);

        public void Add(Entity e)
        {
            _list.Add(e);
            HasChanges = true;
        }

        public bool HasChanges;

        public void Commit(List<BaseSystem> systems)
        {
            foreach (var e in _list)
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

            _list.Clear();
            HasChanges = false;
        }
    }
}