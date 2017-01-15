using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Ecs.Changes;

namespace Misana.Core.Components
{
    public class EntityColliderComponent : Component<EntityColliderComponent>
    {
        public override void Reset()
        {
            base.Reset();
            Mass = 50f;
            Fixed = false;
            Blocked = false;
            CollisionEffects.Clear();
        }

        public float Mass = 50f;
        public bool Fixed;

        public List<CollisionEffect> CollisionEffects = new List<CollisionEffect>(2);
        public bool Blocked;

        public override void CopyTo(EntityColliderComponent other)
        {
            other.Mass = Mass;
            other.Fixed = Fixed;
            other.Blocked = Blocked;
            other.CollisionEffects = new List<CollisionEffect>(CollisionEffects); //TODO: :(
        }
    }

    public abstract class CollisionEffect
    {
        public enum ApplicableTo : byte
        {
            None = 0,
            Self,
            Other,
            Both
        }


        public ApplicableTo ApplyTo;
        public TimeSpan Debounce;
        public TimeSpan CoolDown;

        protected TimeSpan LastExecution;
        protected Dictionary<int, TimeSpan> RecentExecutions = new Dictionary<int, TimeSpan>();

        public abstract void Apply(EntityManager manager, Entity self, Entity other);
    }

    public class CustomCodeCollisionEffect : CollisionEffect
    {
        private readonly Action<EntityManager, Entity, Entity> _action;

        public CustomCodeCollisionEffect(Action<EntityManager, Entity, Entity> action)
        {
            _action = action;
        }

        public override void Apply(EntityManager manager, Entity self, Entity other)
        {
            if (Debounce != TimeSpan.Zero)
            {
                TimeSpan val;

                if (RecentExecutions.TryGetValue(self.Id, out val))
                {
                    if (manager.GameTime.TotalTime - val > Debounce)
                        RecentExecutions.Remove(self.Id);
                    else
                    {
                        return;
                    }
                }

                RecentExecutions[self.Id] = manager.GameTime.TotalTime;
            }

            _action(manager, self, other);
        }
    }

    public class SimpleAddComponentCollisionEffect<T> : CollisionEffect where T : Component, new()
    {
        public SimpleAddComponentCollisionEffect(T template)
        {
            Template = template;
        }

        public readonly T Template;

        private readonly object _lockObj = new object();

        public override void Apply(EntityManager manager, Entity self, Entity other)
        {
            if (LastExecution != TimeSpan.Zero && CoolDown != TimeSpan.Zero)
            {
                if(manager.GameTime.TotalTime - LastExecution < CoolDown)
                    return;
            }

            if (ApplyTo == ApplicableTo.Self || ApplyTo == ApplicableTo.Both)
            {
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
                                return;
                            }
                        }

                        RecentExecutions[self.Id] = manager.GameTime.TotalTime;
                    }
                }

                var a = ComponentRegistry<T>.TakeManagedAddition();
                a.EntityId = self.Id;
                a.Template = Template;

                manager.Change(a);
            }

            if (ApplyTo == ApplicableTo.Other || ApplyTo == ApplicableTo.Both)
            {
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
                                return;
                            }
                        }

                        RecentExecutions[other.Id] = manager.GameTime.TotalTime;
                    }
                }

                var a = ComponentRegistry<T>.TakeManagedAddition();
                a.EntityId = other.Id;
                a.Template = Template;

                manager.Change(a);

            }

            LastExecution = manager.GameTime.TotalTime;
        }
    }
}
