﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Entities
{
    public class MultiEvent : OnEvent
    {
        private EventCondition _condition;
        private OnEvent[] _events;
        public MultiEvent(EventCondition condition, params OnEvent[] events)
        {
            _condition = condition;
            _events = events;
        }

        public MultiEvent(){}

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
            _events = new OnEvent[lenght];
            for (int i = 0; i < lenght; i++)
            {
                var typeName = br.ReadString();
                var @event = (OnEvent) Activator.CreateInstance(Type.GetType(typeName));
                _events[i] = @event;
            }
        }

        public override void Apply(EntityManager manager, Entity self, Entity other, ISimulation simulation)
        {
            if (!_condition.Test(manager, self, other, simulation))
                return;

            base.Apply(manager, self, other, simulation);
        }

        internal override async Task<bool> ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, ISimulation world)
        {
            var applied = false;

            for (int i = 0; i < _events.Length; i++)
            {
                if(
                    (targetIsSelf && (_events[i].ApplyTo == ApplicableTo.Self || _events[i].ApplyTo == ApplicableTo.Both) )
                || (!targetIsSelf &&  (_events[i].ApplyTo == ApplicableTo.Other || _events[i].ApplyTo == ApplicableTo.Both) )
                )

                    applied |= await _events[i].ApplyToEntity(manager, targetIsSelf, target, world);
            }

            return applied;
        }

        public override OnEvent Copy()
        {
            return new MultiEvent(_condition?.Copy(), _events.Select(e => e.Copy()).ToArray());
        }
    }
}