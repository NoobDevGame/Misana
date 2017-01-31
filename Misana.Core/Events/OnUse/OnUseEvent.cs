using System;
using Misana.Core.Ecs;

namespace Misana.Core.Events.OnUse
{
    public abstract class OnUseEvent : EventDefinition
    {
        public TimeSpan CoolDown;
        protected TimeSpan LastExecution;

        protected virtual bool CanApply(EntityManager manager, Entity target, ISimulation simulation)
        {
            return true;
        }

        private readonly object _lockObj = new object();


        public virtual void Apply(EntityManager manager, Entity self, Vector2 target, ISimulation simulation)
        {
            if (RunsOn != RunsOn.Both && (byte)manager.Mode != (byte)RunsOn)
                return;

            if (LastExecution != TimeSpan.Zero && CoolDown != TimeSpan.Zero)
            {
                if (manager.GameTime.TotalTime - LastExecution < CoolDown)
                    return;
            }

            var applied = ApplyToTarget(manager, self, target, simulation);

            if(applied)
                LastExecution = manager.GameTime.TotalTime;
        }

        public abstract OnUseEvent Copy();

        protected abstract bool ApplyToTarget(EntityManager manager, Entity self, Vector2 target, ISimulation simulation);
    }
}