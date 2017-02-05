using System;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Misana.Core.Ecs;
using Misana.Core.Effects;
using Misana.Serialization;

namespace Misana.Core.Events.Entities
{
    public class MultiEvent : OnEvent
    {
        public EventCondition Condition;
        public OnEvent[] Events;
        public MultiEvent(EventCondition condition, params OnEvent[] events)
        {
            Condition = condition;
            Events = events;
        }

        public MultiEvent(){}

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write((byte)RunsOn);
            bw.Write(Condition != null);

            if (Condition != null)
            {
                bw.Write(Condition.GetType().AssemblyQualifiedName);
                Condition.Serialize(version,bw);
            }

            bw.Write(Events.Length);
            foreach (var @event in Events)
            {
                bw.Write(@event.GetType().AssemblyQualifiedName);
                @event.Serialize(version,bw);
            }
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            RunsOn = (RunsOn)br.ReadByte();
            var existCondition = br.ReadBoolean();

            if (existCondition)
            {
                var typeName = br.ReadString();
                Condition = (EventCondition) Activator.CreateInstance(Type.GetType(typeName));
                Condition.Deserialize(version,br);
            }

            var lenght = br.ReadInt32();
            Events = new OnEvent[lenght];
            for (int i = 0; i < lenght; i++)
            {
                var typeName = br.ReadString();
                var @event = (OnEvent) Activator.CreateInstance(Type.GetType(typeName));
                Events[i] = @event;
            }
        }

        public override void Apply(EntityManager manager, Entity self, Entity other, ISimulation simulation)
        {
            if (RunsOn != RunsOn.Both && (byte)manager.Mode != (byte)RunsOn)
                return;

            if (!Condition.Test(manager, self, other, simulation))
                return;

            base.Apply(manager, self, other, simulation);
        }

        internal override bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, ISimulation world)
        {
            var applied = false;

            for (int i = 0; i < Events.Length; i++)
            {
                if(
                    (targetIsSelf && (Events[i].ApplyTo == ApplicableTo.Self || Events[i].ApplyTo == ApplicableTo.Both) )
                || (!targetIsSelf &&  (Events[i].ApplyTo == ApplicableTo.Other || Events[i].ApplyTo == ApplicableTo.Both) )
                )

                    applied |= Events[i].ApplyToEntity(manager, targetIsSelf, target, world);
            }

            return applied;
        }

        public override OnEvent Copy()
        {
            return new MultiEvent(Condition?.Copy(), Events.Select(e => e.Copy()).ToArray());
        }

        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(0, ref target, ref pos);
            Serializes<MultiEvent>.Serialize(this, ref target, ref pos);
        }

        public static void InitializeSerialization()
        {
            Serializes<MultiEvent>.Serialize = (MultiEvent item, ref byte[] bytes, ref int index) => {
                Serializes<RunsOn>.Serialize(item.RunsOn, ref bytes, ref index);
                Serializes<ApplicableTo>.Serialize(item.ApplyTo, ref bytes, ref index);
                Serializes<TimeSpan>.Serialize(item.Debounce, ref bytes, ref index);
                Serializes<TimeSpan>.Serialize(item.CoolDown, ref bytes, ref index);
                if (item.Condition == null)
                {
                    Serializer.WriteBoolean(false, ref bytes, ref index);
                }
                else
                {
                    Serializer.WriteBoolean(true, ref bytes, ref index);
                    item.Condition.Serialize(ref bytes, ref index);
                }

                Serializer.WriteInt32(item.Events.Length, ref bytes,ref index);
                for (int i = 0; i < item.Events.Length; i++)
                {
                    item.Events[0].Serialize(ref bytes, ref index);
                }
            };

            Serializes<MultiEvent>.Deserialize = (byte[] bytes, ref int index) => {
                var item = new MultiEvent();
                item.RunsOn = Serializes<RunsOn>.Deserialize(bytes, ref index);
                item.ApplyTo = Serializes<ApplicableTo>.Deserialize(bytes, ref index);
                item.Debounce = Serializes<TimeSpan>.Deserialize(bytes, ref index);
                item.CoolDown = Serializes<TimeSpan>.Deserialize(bytes, ref index);
                if (Deserializer.ReadBoolean(bytes, ref index))
                    item.Condition = Serializes<EventCondition>.Deserialize(bytes, ref index);

                var cnt = Deserializer.ReadInt32(bytes, ref index);
                item.Events = new OnEvent[cnt];
                for (int i = 0; i < cnt; i++)
                {
                    item.Events[i] = Serializes<OnEvent>.Deserialize(bytes, ref index);
                }

                return item;
            };

            Deserializers[0] = (byte[] bytes, ref int index)
                => (OnEvent) Serializes<MultiEvent>.Deserialize(bytes, ref index);
        }
    }
}