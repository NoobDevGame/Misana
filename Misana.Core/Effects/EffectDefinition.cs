using System;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Effects.BaseEffects;
using Misana.Serialization;

namespace Misana.Core.Effects
{
    public abstract class EffectDefinition
    {
        protected static Deserialize<EffectDefinition>[] Deserializers;
        public abstract void Apply(Entity entity, ISimulation simulation);

        public abstract void Serialize(Version version,BinaryWriter bw);
        public abstract void Deserialize(Version version, BinaryReader br);
        public abstract void Serialize(ref byte[] target, ref int pos);

        public static void Initialize()
        {
            Deserializers = new Deserialize<EffectDefinition>[4];
            DamageEffect.InitializeSerialization();
            RemoveSelfEffect.InitializeSerialization();
            SetEntityFlagEffect.InitializeSerialization();
            TeleportEffect.InitializeSerialization();

            Serializes<EffectDefinition>.Deserialize = (byte[] bytes, ref int index) => {
                var idx = Deserializer.ReadInt32(bytes, ref index);
                return Deserializers[idx](bytes, ref index);
            };

            Serializes<EffectDefinition>.Serialize = (EffectDefinition item, ref byte[] bytes, ref int index) => {
                item.Serialize(ref bytes, ref index);
            };
            EffectCondition.Initialize();
        }
    }
}