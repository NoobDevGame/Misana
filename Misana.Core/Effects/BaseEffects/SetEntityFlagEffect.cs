using System;
using System.IO;
using Misana.Core.Components.Events;
using Misana.Core.Ecs;
using Misana.Core.Events;
using Misana.Serialization;

namespace Misana.Core.Effects.BaseEffects
{
    public class SetEntityFlagEffect : EffectDefinition
    {

        public string Name { get; set; } = "Unnamed";

        public SetEntityFlagEffect()
        {

        }

        public SetEntityFlagEffect(string name)
        {
            Name = name;
        }

        public override void Apply(Entity entity, ISimulation simulation)
        {
            var flagComponent = entity.Get<EntityFlagComponent>();

            flagComponent?.Set(Name);
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Name);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Name = br.ReadString();
        }

        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(2, ref target, ref pos);
            Serializes<SetEntityFlagEffect>.Serialize(this, ref target, ref pos);
        }

        public static void InitializeSerialization()
        {
            Serializes<SetEntityFlagEffect>.Serialize = (SetEntityFlagEffect item, ref byte[] bytes, ref int index) => {
                Serializer.WriteString(item.Name, ref bytes, ref index);
            };

            Serializes<SetEntityFlagEffect>.Deserialize =(byte[] bytes, ref int index) =>
                new SetEntityFlagEffect(Deserializer.ReadString(bytes, ref index));

            Deserializers[2] = (byte[] bytes, ref int index)
                => (EffectDefinition) Serializes<SetEntityFlagEffect>.Deserialize(bytes, ref index);
        }
    }
}