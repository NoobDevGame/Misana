using System.Collections.Generic;

namespace Misana.Core.Ecs.Changes
{
    internal class EntitiesToRemove
    {
        private class EntityRemoval
        {
            public Entity Entity;
            public bool Detach;

            public EntityRemoval(Entity entity, bool detach)
            {
                Entity = entity;
                Detach = detach;
            }
        }

        private readonly List<EntityRemoval> _list = new List<EntityRemoval>(16);

        public void Add(Entity e, bool detach = false)
        {
            _list.Add(new EntityRemoval(e, detach));
            HasChanges = true;
        }

        public bool HasChanges;

        public void Commit(List<BaseSystem> systems)
        {
            foreach (var e in _list)
            {
                foreach (var s in systems)
                    s.EntityRemoved(e.Entity);

                if (!e.Detach)
                {
                    ComponentArrayPool.Release(e.Entity.Components);
                    e.Entity.Components = null;
                }
            }

            _list.Clear();
            HasChanges = false;
        }
    }
}