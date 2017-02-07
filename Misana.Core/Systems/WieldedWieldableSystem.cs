using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class WieldedWieldableSystem : BaseSystemR2<WieldedComponent, WieldableComponent>
    {
        private ISimulation _simulation;

        public void ChangeSimulation(ISimulation simulation)
        {
            _simulation = simulation;
        }

        protected override void Add(Entity e, int index)
        {
            base.Add(e, index);
            e.Get<WieldedComponent>().Spawner = e.Get<SpawnerComponent>();

        }

        protected override void Remove(int index, int? swapWith)
        {
            var wc = Entities[index].Get<WieldedComponent>();

            if (wc != null)
                wc.Spawner = null;

            base.Remove(index, swapWith);
        }

        protected override void Update(Entity e, WieldedComponent r1, WieldableComponent r2)
        {
            if (r1.Spawner != null)
            {
                r1.Spawner.Active = r1.Use;

                if(r1.Use)
                    r1.Spawner.SpawnDirection = r1.ParentFacing;
            }

            if (!r1.Use)
            {
                return;
            }

            foreach(var ev in r2.OnUseEvents)
                ev.Apply(Manager, e, r1.ParentFacing, _simulation);
        }
    }
}