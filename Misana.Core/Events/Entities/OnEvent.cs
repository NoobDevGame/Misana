using System;
using System.Collections.Generic;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Entities
{
    public abstract class OnEvent : EventDefinition
    {
        public ApplicableTo ApplyTo;
        public TimeSpan Debounce;
        public TimeSpan CoolDown;

        protected TimeSpan LastExecution;
        protected Dictionary<int, TimeSpan> RecentExecutions = new Dictionary<int, TimeSpan>();

        protected virtual bool CanApply(EntityManager manager, Ecs.Entity target, ISimulation simulation)
        {
            return true;
        }

        private readonly object _lockObj = new object();
        
        internal abstract bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Ecs.Entity target, ISimulation simulation);

        public virtual void Apply(EntityManager manager, Ecs.Entity self, Ecs.Entity other, ISimulation simulation)
        {
            if (LastExecution != TimeSpan.Zero && CoolDown != TimeSpan.Zero)
            {
                if (manager.GameTime.TotalTime - LastExecution < CoolDown)
                    return;
            }

            var applied = false;

            if (ApplyTo == ApplicableTo.Self || ApplyTo == ApplicableTo.Both)
            {
                if (CanApply(manager, self, simulation))
                {
                    var apply = true;
                    if (Debounce != TimeSpan.Zero)
                    {
                        TimeSpan val;
                        lock (_lockObj)
                        {
                            if (RecentExecutions.TryGetValue(self.Id, out val))
                            {
                                if (manager.GameTime.TotalTime - val > Debounce)
                                    RecentExecutions.Remove(self.Id);
                                else
                                {
                                    apply = false;
                                }
                            }

                            if(apply)
                                RecentExecutions[self.Id] = manager.GameTime.TotalTime;
                        }
                    }

                    if(apply)
                        applied = ApplyToEntity(manager, true, self, simulation);
                }
            }

            if (ApplyTo == ApplicableTo.Other || ApplyTo == ApplicableTo.Both)
            {
                if (CanApply(manager, other, simulation))
                {
                    var apply = true;
                    if (Debounce != TimeSpan.Zero)
                    {
                        TimeSpan val;
                        lock (_lockObj)
                        {
                            if (RecentExecutions.TryGetValue(other.Id, out val))
                            {
                                if (manager.GameTime.TotalTime - val > Debounce)
                                    RecentExecutions.Remove(other.Id);
                                else
                                {
                                    apply = false;
                                }
                            }

                            if(apply)
                                RecentExecutions[other.Id] = manager.GameTime.TotalTime;
                        }
                    }

                    if (apply)
                    {
                        applied = ApplyToEntity(manager, false, other, simulation) || applied;
                    }
                }
            }

            if(applied)
                LastExecution = manager.GameTime.TotalTime;
        }

        public abstract OnEvent Copy();
    }
}