using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        protected List<KeyValuePair<int, TimeSpan>> RecentExecutions = new List<KeyValuePair<int, TimeSpan>>();

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
                    for (int i = 0; i < RecentExecutions.Count; i++)
                    {
                        if (manager.GameTime.TotalTime - RecentExecutions[i].Value > Debounce)
                        {
                            RecentExecutions.RemoveAt(i--);
                            continue;
                        }

                        if(RecentExecutions[i].Key == self.Id)
                            return;
                    }

                    RecentExecutions.Add(new KeyValuePair<int, TimeSpan>(self.Id, manager.GameTime.TotalTime));
                }

                T t = ComponentRegistry<T>.Take();
                Template.CopyTo(t);
                manager.Add(self, t, false);
            }

            if (ApplyTo == ApplicableTo.Other || ApplyTo == ApplicableTo.Both)
            {
                if (Debounce != TimeSpan.Zero)
                {
                    for (int i = 0; i < RecentExecutions.Count; i++)
                    {
                        if (manager.GameTime.TotalTime - RecentExecutions[i].Value > Debounce)
                        {
                            RecentExecutions.RemoveAt(i--);
                            continue;
                        }

                        if (RecentExecutions[i].Key == other.Id)
                            return;
                    }

                    RecentExecutions.Add(new KeyValuePair<int, TimeSpan>(other.Id, manager.GameTime.TotalTime));
                }

                T t = ComponentRegistry<T>.Take();
                Template.CopyTo(t);
                manager.Add(other, t, false);
            }

            LastExecution = manager.GameTime.TotalTime;
        }
    }
}
