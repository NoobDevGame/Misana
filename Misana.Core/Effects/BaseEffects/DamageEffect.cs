using System;
using System.IO;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Serialization;

namespace Misana.Core.Effects.BaseEffects
{
    public class DamageEffect : EffectDefinition
    {
        public float Damage { get; set; }

        public DamageEffect()
        {

        }

        public DamageEffect(float damage)
        {
            Damage = damage;
        }

        public override void Apply(Entity entity, ISimulation simulation)
        {
            var healthComponet = entity.Get<HealthComponent>();

            if (healthComponet != null)
            {
                ApplyLocally(new OnDamageEffectMessage(entity.Id,Damage), entity, healthComponet, simulation);
            }
        }

        public static void ApplyLocally(OnDamageEffectMessage effect, Entity e, HealthComponent health, ISimulation simulation)
        {
            ApplyFromRemote(effect, e, health, simulation);
            simulation.Entities.NoteForSend(effect);
        }

        public static void ApplyFromRemote(OnDamageEffectMessage message, Entity e, HealthComponent health, ISimulation simulation)
        {
            health.Current -= message.Damage;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Damage);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Damage = br.ReadSingle();
        }

        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(0, ref target, ref pos);
            Serializes<DamageEffect>.Serialize(this, ref target, ref pos);
        }

        public static void InitializeSerialization()
        {
            Serializes<DamageEffect>.Serialize = (DamageEffect item, ref byte[] bytes, ref int index) => {
                Serializer.WriteSingle(item.Damage, ref bytes, ref index);
            };

            Serializes<DamageEffect>.Deserialize =(byte[] bytes, ref int index) =>
                new DamageEffect(
                    Deserializer.ReadSingle(bytes, ref index)
                );

            Deserializers[0] = (byte[] bytes, ref int index)
                => (EffectDefinition) Serializes<DamageEffect>.Deserialize(bytes, ref index);
        }
    }
}