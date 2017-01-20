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

        protected override void Update(Entity e, WieldedComponent r1, WieldableComponent r2)
        {
            if (!r1.Use)
                return;

            foreach(var ev in r2.OnUseEvents)
                ev.Apply(Manager, e, r1.ParentFacing, _simulation);
        }
    }
}