using System;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Events.Conditions;
using Misana.Serialization;

namespace Misana.Core.Events
{
    public abstract class EventCondition
    {
        public abstract bool Test (EntityManager manager, Entity self, Entity other, ISimulation simulation);

        public abstract void Serialize(Version version,BinaryWriter bw);
        public abstract void Deserialize(Version version, BinaryReader br);

        public abstract EventCondition Copy();

        public abstract void Serialize(ref byte[] bytes, ref int index);

        public static Deserialize<EventCondition>[] Deserializers;

        public static void Initialize()
        {
            Deserializers = new Deserialize<EventCondition>[1];

            FlagCondition.InitializeSerialization();

            Serializes<EventCondition>.Deserialize = (byte[] bytes, ref int index) => {
                var idx = Deserializer.ReadInt32(bytes, ref index);
                return Deserializers[idx](bytes, ref index);
            };

            Serializes<EventCondition>.Serialize = (EventCondition item, ref byte[] bytes, ref int index) => {
                item.Serialize(ref bytes, ref index);
            };
        }
    }
}