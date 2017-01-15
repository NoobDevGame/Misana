using System.Collections.Generic;

namespace Misana.Core.Ecs.Changes
{
    internal class EntitiesToRemove
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
                foreach (var s in systems)
                    s.EntityRemoved(e);

                ComponentArrayPool.Release(e.Components);
                e.Components = null;
            }

            HasChanges = false;
        }
    }
}