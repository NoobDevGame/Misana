﻿using System;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Effects.Conditions;
using Misana.Serialization;

namespace Misana.Core.Effects
{
    public abstract class EffectCondition
    {
        public abstract bool Test (Entity entity,ISimulation simulation);

        public abstract void Serialize(Version version,BinaryWriter bw);
        public abstract void Deserialize(Version version, BinaryReader br);

        public abstract void Serialize(ref byte[] bytes, ref int index);

        public static Deserialize<EffectCondition>[] Deserializers;

        public static void Initialize()
        {
            Deserializers = new Deserialize<EffectCondition>[1];

            FlagCondition.InitializeSerialization();

            Serializes<EffectCondition>.Deserialize = (byte[] bytes, ref int index) => {
                var idx = Deserializer.ReadInt32(bytes, ref index);
                return Deserializers[idx](bytes, ref index);
            };

            Serializes<EffectCondition>.Serialize = (EffectCondition item, ref byte[] bytes, ref int index) => {
                item.Serialize(ref bytes, ref index);
            };
        }
    }
}