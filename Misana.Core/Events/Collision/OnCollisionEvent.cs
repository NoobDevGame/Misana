using System;
using System.Collections.Generic;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Collision
{
    public abstract class OnCollisionEvent : EventDefinition
    {
        public ApplicableTo ApplyTo;
        public TimeSpan Debounce;
        public TimeSpan CoolDown;

        protected TimeSpan LastExecution;
        protected Dictionary<int, TimeSpan> RecentExecutions = new Dictionary<int, TimeSpan>();

        protected virtual bool CanApply(EntityManager manager, Entity target, World world)
        {
            return true;
        }

        private readonly object _lockObj = new object();

        internal abstract bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, World world);

        public virtual void Apply(EntityManager manager, Entity self, Entity other, World world)
        {
            if (LastExecution != TimeSpan.Zero && CoolDown != TimeSpan.Zero)
            {
                if (manager.GameTime.TotalTime - LastExecution < CoolDown)
                    return;
            }

            var applied = false;

            if (ApplyTo == ApplicableTo.Self || ApplyTo == ApplicableTo.Both)
            {
                if (CanApply(manager, self, world))
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
                        applied = ApplyToEntity(manager, true, self, world);
                }
            }

            if (ApplyTo == ApplicableTo.Other || ApplyTo == ApplicableTo.Both)
            {
                if (CanApply(manager, other, world))
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
                        applied = ApplyToEntity(manager, false, other, world) || applied;
                    }
                }
            }

            if(applied)
                LastExecution = manager.GameTime.TotalTime;
        }
    }
}