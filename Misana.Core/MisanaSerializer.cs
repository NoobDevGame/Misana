using Misana.Core.Ecs;
using Misana.Core.Effects;
using Misana.Core.Events;
using Misana.Core.Events.Entities;
using Misana.Serialization;

namespace Misana.Core
{
    public class MisanaSerializer
    {
        public static void Initialize()
        {
            Serializes<Vector2>.Serialize = (Vector2 item, ref byte[] bytes, ref int index) => {
                Serializer.WriteSingle(item.X, ref bytes, ref index);
                Serializer.WriteSingle(item.Y, ref bytes, ref index);
            };

            Serializes<Vector2>.Deserialize  = (byte[] bytes, ref int index) =>
                new Vector2(Deserializer.ReadSingle(bytes, ref index), Deserializer.ReadSingle(bytes, ref index));

            Serializes<Index2>.Serialize = (Index2 item, ref byte[] bytes, ref int index) => {
                Serializer.WriteInt32(item.X, ref bytes, ref index);
                Serializer.WriteInt32(item.Y, ref bytes, ref index);
            };

            Serializes<Index2>.Deserialize  = (byte[] bytes, ref int index) =>
                new Index2(Deserializer.ReadInt32(bytes, ref index), Deserializer.ReadInt32(bytes, ref index));

            Serializes<RunsOn>.Deserialize = (byte[] bytes, ref int index) =>
                (RunsOn)Deserializer.ReadByte(bytes, ref index);

            Serializes<RunsOn>.Serialize = (RunsOn item, ref byte[] bytes, ref int index) => {
                Serializer.WriteByte((byte)item, ref bytes, ref index);
            };

            Serializes<ApplicableTo>.Deserialize = (byte[] bytes, ref int index) =>
                (ApplicableTo)Deserializer.ReadByte(bytes, ref index);

            Serializes<ApplicableTo>.Serialize = (ApplicableTo item, ref byte[] bytes, ref int index) => {
                Serializer.WriteByte((byte)item, ref bytes, ref index);
            };

            Serializes<AttachmentType>.Deserialize = (byte[] bytes, ref int index) =>
                (AttachmentType)Deserializer.ReadByte(bytes, ref index);

            Serializes<AttachmentType>.Serialize = (AttachmentType item, ref byte[] bytes, ref int index) => {
                Serializer.WriteByte((byte)item, ref bytes, ref index);
            };

            Serializes<AttachedEntity>.Deserialize = (byte[] bytes, ref int index) => {
                var item = new AttachedEntity();
                item.AttachmentType = Serializes<AttachmentType>.Deserialize(bytes, ref index);
                item.Builder = Serializes<EntityBuilder>.Deserialize(bytes, ref index);
                return item;
            };

            Serializes<AttachedEntity>.Serialize = (AttachedEntity item, ref byte[] bytes, ref int index) => {
                Serializes<AttachmentType>.Serialize(item.AttachmentType, ref bytes, ref index);
                Serializes<EntityBuilder>.Serialize(item.Builder, ref bytes, ref index);
            };

            EventDefinition.Initialize();
            EffectDefinition.Initialize();
        }
    }
}