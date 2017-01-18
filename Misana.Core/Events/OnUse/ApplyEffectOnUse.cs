using System;
using System.Collections.Generic;
using Misana.Core.Ecs;
using Misana.Core.Effects;

namespace Misana.Core.Events.OnUse
{
    public abstract class OnUseEvent
    {
        public TimeSpan CoolDown;
        protected TimeSpan LastExecution;

        protected virtual bool CanApply(EntityManager manager, Entity target, World world)
        {
            return true;
        }

        private readonly object _lockObj = new object();


        public virtual void Apply(EntityManager manager, Entity self, Vector2 target, World world)
        {
            if (LastExecution != TimeSpan.Zero && CoolDown != TimeSpan.Zero)
            {
                if (manager.GameTime.TotalTime - LastExecution < CoolDown)
                    return;
            }

            var applied = ApplyToTarget(manager, self, target, world);

            if(applied)
                LastExecution = manager.GameTime.TotalTime;
        }

        protected abstract bool ApplyToTarget(EntityManager manager, Entity self, Vector2 target, World world);
    }

    public class ApplyEffectOnUseEvent : OnUseEvent
    {
        private readonly EffectDefinition _eff;

        public ApplyEffectOnUseEvent(EffectDefinition eff)
        {
            _eff = eff;
        }

        protected override bool ApplyToTarget(EntityManager manager, Entity self, Vector2 target, World world)
        {
            _eff.Apply(self, world);
            return true;
        }
    }
}