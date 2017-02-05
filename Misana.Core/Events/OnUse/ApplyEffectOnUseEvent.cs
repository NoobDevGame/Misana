using System;
using System.Collections.Generic;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Effects;
using Misana.Core.Events.Entities;
using Misana.Serialization;

namespace Misana.Core.Events.OnUse
{
    public class ApplyEffectOnUseEvent : OnUseEvent
    {
        public EffectDefinition Effect;

        public ApplyEffectOnUseEvent()
        {

        }

        public ApplyEffectOnUseEvent(EffectDefinition effect)
        {
            Effect = effect;
        }

        protected override bool ApplyToTarget(EntityManager manager, Entity self, Vector2 target, ISimulation simulation)
        {
            Effect?.Apply(self, simulation);
            return true;
        }

        public override OnUseEvent Copy()
        {
            return new ApplyEffectOnUseEvent(Effect) { CoolDown = CoolDown };
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write((byte)RunsOn);
            bw.Write(Effect.GetType().AssemblyQualifiedName);
            Effect.Serialize(version,bw);
            bw.Write(CoolDown.TotalMilliseconds);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            RunsOn = (RunsOn)br.ReadByte();
            var name = br.ReadString();
            var type = Type.GetType(name);
            Effect = (EffectDefinition) Activator.CreateInstance(type);
            Effect.Deserialize(version,br);
            CoolDown = TimeSpan.FromMilliseconds(br.ReadDouble());
        }

        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(0, ref target, ref pos);
            Serializes<ApplyEffectOnUseEvent>.Serialize(this, ref target, ref pos);
        }

        public static void InitializeSerialization()
        {
            Serializes<ApplyEffectOnUseEvent>.Serialize = (ApplyEffectOnUseEvent item, ref byte[] bytes, ref int index) => {
                Serializes<RunsOn>.Serialize(item.RunsOn, ref bytes, ref index);
                Serializes<TimeSpan>.Serialize(item.CoolDown, ref bytes, ref index);
                item.Effect.Serialize(ref bytes, ref index);
            };

            Serializes<ApplyEffectOnUseEvent>.Deserialize = (byte[] bytes, ref int index) => {
                var item = new ApplyEffectOnUseEvent();
                item.RunsOn = Serializes<RunsOn>.Deserialize(bytes, ref index);
                item.CoolDown = Serializes<TimeSpan>.Deserialize(bytes, ref index);
                item.Effect = Serializes<EffectDefinition>.Deserialize(bytes, ref index);
                return item;
            };

            Deserializers[0] = (byte[] bytes, ref int index)
                => (OnUseEvent) Serializes<ApplyEffectOnUseEvent>.Deserialize(bytes, ref index);
        }
    }
}