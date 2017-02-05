using System;
using System.IO;
using Misana.Core.Components.Events;
using Misana.Core.Ecs;
using Misana.Serialization;

namespace Misana.Core.Effects.Conditions
{
    public class FlagCondition : EffectCondition
    {
        public bool Not { get; set; }
        public string Name { get; set; }

        public FlagCondition()
        {

        }

        public FlagCondition(string name)
        {
            Name = name;
        }

        public FlagCondition(string name,bool not)
        {
            Name = name;
            Not = not;
        }

        public override bool Test(Entity entity, ISimulation simulation)
        {
            var flagComponent = entity.Get<EntityFlagComponent>();

            if (flagComponent == null)
                return true;

            var value = flagComponent.Get(Name);

            if (Not)
                return !value;

            return value;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Name);
            bw.Write(Not);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Name = br.ReadString();
            Not = br.ReadBoolean();
        }
        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(0, ref target, ref pos);
            Serializes<FlagCondition>.Serialize(this, ref target, ref pos);
        }


        public static void InitializeSerialization()
        {
            Serializes<FlagCondition>.Serialize = (FlagCondition item, ref byte[] bytes, ref int index) => {
                Serializer.WriteBoolean(item.Not, ref bytes, ref index);
                Serializer.WriteString(item.Name, ref bytes, ref index);
            };

            Serializes<FlagCondition>.Deserialize = (byte[] bytes, ref int index) => {
                var item = new FlagCondition();
                item.Not = Deserializer.ReadBoolean(bytes, ref index);
                item.Name = Deserializer.ReadString(bytes, ref index);
                return item;
            };

            Deserializers[0] = (byte[] bytes, ref int index)
                => (EffectCondition) Serializes<FlagCondition>.Deserialize(bytes, ref index);
        }

    }
}