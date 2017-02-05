using System;
using System.IO;
using System.Threading.Tasks;
using Misana.Core.Ecs;
using Misana.Core.Effects;
using Misana.Core.Effects.BaseEffects;
using Misana.Serialization;

namespace Misana.Core.Events.Entities
{
    public class ApplyEffectEvent : OnEvent
    {
        public EffectDefinition Effect;
        public EffectCondition Condition;

        public ApplyEffectEvent()
        {

        }

        public ApplyEffectEvent(EffectDefinition deff, EffectCondition condition = null)
        {
            Effect = deff;
            Condition = condition;
        }


        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write((byte)RunsOn);
            if (Condition != null)
            {
                bw.Write(true);
                bw.Write(Condition.GetType().AssemblyQualifiedName);
                Condition.Serialize(version, bw);
            }
            else
            {
                bw.Write(false);
            }

            bw.Write(Effect.GetType().AssemblyQualifiedName);
            Effect.Serialize(version, bw);

            bw.Write((byte)ApplyTo);
            bw.Write(CoolDown.TotalMilliseconds);
            bw.Write(Debounce.TotalMilliseconds);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            RunsOn = (RunsOn) br.ReadByte();
            var conditionExist = br.ReadBoolean();
            if (conditionExist)
            {
                var typeName = br.ReadString();
                var condition = (EffectCondition)Activator.CreateInstance(Type.GetType(typeName));
                condition.Deserialize(version, br);
                Condition = condition;
            }

            var typeName2 = br.ReadString();
            Effect = (EffectDefinition)Activator.CreateInstance(Type.GetType(typeName2));
            Effect.Deserialize(version, br);

            ApplyTo = (ApplicableTo) br.ReadByte();
            CoolDown = TimeSpan.FromMilliseconds(br.ReadDouble());
            Debounce = TimeSpan.FromMilliseconds(br.ReadDouble());
        }

        protected override bool CanApply(EntityManager manager, Entity target, ISimulation simulation)
        {
            return Condition?.Test(target, simulation) ?? true;
        }

        internal override bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, ISimulation simulation)
        {
            Effect.Apply(target, simulation);
            return true;
        }

        public override OnEvent Copy()
        {
            return new ApplyEffectEvent(Effect, Condition) { ApplyTo = ApplyTo, RunsOn = RunsOn, Condition = Condition, CoolDown = CoolDown };
        }

        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(1, ref target, ref pos);
            Serializes<ApplyEffectEvent>.Serialize(this, ref target, ref pos);
        }

        public static void InitializeSerialization()
        {
            Serializes<ApplyEffectEvent>.Serialize = (ApplyEffectEvent item, ref byte[] bytes, ref int index) => {
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

                item.Effect.Serialize(ref bytes, ref index);
            };

            Serializes<ApplyEffectEvent>.Deserialize = (byte[] bytes, ref int index) => {
                var item = new ApplyEffectEvent();
                item.RunsOn = Serializes<RunsOn>.Deserialize(bytes, ref index);
                item.ApplyTo = Serializes<ApplicableTo>.Deserialize(bytes, ref index);
                item.Debounce = Serializes<TimeSpan>.Deserialize(bytes, ref index);
                item.CoolDown = Serializes<TimeSpan>.Deserialize(bytes, ref index);
                if (Deserializer.ReadBoolean(bytes, ref index))
                    item.Condition = Serializes<EffectCondition>.Deserialize(bytes, ref index);
                item.Effect = Serializes<EffectDefinition>.Deserialize(bytes, ref index);
                return item;
            };

            Deserializers[1] = (byte[] bytes, ref int index)
                => (OnEvent) Serializes<ApplyEffectEvent>.Deserialize(bytes, ref index);
        }
    }
}