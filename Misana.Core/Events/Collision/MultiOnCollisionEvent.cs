using System;
using System.ComponentModel;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Collision
{
    public class MultiOnCollisionEvent : OnCollisionEvent
    {
        private EventCondition _condition;
        private OnCollisionEvent[] _events;
        public MultiOnCollisionEvent(EventCondition condition, params OnCollisionEvent[] events)
        {
            _condition = condition;
            _events = events;
        }

        public MultiOnCollisionEvent(){}

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(_condition != null);

            if (_condition != null)
            {
                bw.Write(_condition.GetType().AssemblyQualifiedName);
                _condition.Serialize(version,bw);
            }

            bw.Write(_events.Length);
            foreach (var @event in _events)
            {
                bw.Write(@event.GetType().AssemblyQualifiedName);
                @event.Serialize(version,bw);
            }
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            var existCondition = br.ReadBoolean();

            if (existCondition)
            {
                var typeName = br.ReadString();
                _condition = (EventCondition) Activator.CreateInstance(Type.GetType(typeName));
                _condition.Deserialize(version,br);
            }

            var lenght = br.ReadInt32();
            _events = new OnCollisionEvent[lenght];
            for (int i = 0; i < lenght; i++)
            {
                var typeName = br.ReadString();
                var @event = (OnCollisionEvent) Activator.CreateInstance(Type.GetType(typeName));
                _events[i] = @event;
            }
        }

        public override void Apply(EntityManager manager, Entity self, Entity other, World world)
        {
            if (!_condition.Test(manager, self, other, world))
                return;

            base.Apply(manager, self, other, world);
        }

        internal override bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, World world)
        {
            var applied = false;

            for (int i = 0; i < _events.Length; i++)
            {
                if(
                    (targetIsSelf && (_events[i].ApplyTo == ApplicableTo.Self || _events[i].ApplyTo == ApplicableTo.Both) )
                || (!targetIsSelf &&  (_events[i].ApplyTo == ApplicableTo.Other || _events[i].ApplyTo == ApplicableTo.Both) )
                )

                    applied |= _events[i].ApplyToEntity(manager, targetIsSelf, target, world);
            }

            return applied;
        }
    }
}