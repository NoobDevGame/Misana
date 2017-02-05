using System;
using System.IO;
using Misana.Core.Ecs;
using Misana.Serialization;

namespace Misana.Core.Effects.BaseEffects
{
    public class RemoveSelfEffect : EffectDefinition
    {
        public override void Apply(Entity entity, ISimulation simulation)
        {
            simulation.Entities.RemoveEntity(entity);
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
        }

        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(1, ref target, ref pos);
            Serializes<RemoveSelfEffect>.Serialize(this, ref target, ref pos);
        }

        public static void InitializeSerialization()
        {
            Serializes<RemoveSelfEffect>.Serialize = (RemoveSelfEffect item, ref byte[] bytes, ref int index) => {

            };

            Serializes<RemoveSelfEffect>.Deserialize =(byte[] bytes, ref int index) =>
                new RemoveSelfEffect();

            Deserializers[1] = (byte[] bytes, ref int index)
                => (EffectDefinition) Serializes<RemoveSelfEffect>.Deserialize(bytes, ref index);
        }
    }
}