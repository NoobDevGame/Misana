using System;
using System.IO;
using Misana.Core.Communication.Components;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Core.Events;
using Misana.Core.Events.Entities;
using Misana.Serialization;

namespace Misana.Core.Effects.BaseEffects
{
    public class TeleportEffect : EffectDefinition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int AreaId { get; set; }

        public bool CenterOfBlock { get; set; } = true;

        public TeleportEffect()
        {

        }

        public TeleportEffect(int x, int y , int areaID)
        {
            X = x;
            Y = y;
            AreaId = areaID;
        }

        public TeleportEffect(int x, int y , int areaID, bool center)
        {
            X = x;
            Y = y;
            AreaId = areaID;
            CenterOfBlock = center;
        }


        public override void Apply(Entity entity, ISimulation simulation)
        {
            var positionComponent = entity.Get<TransformComponent>();

            if (positionComponent != null)
            {

                OnTeleportEffectMessage message = new OnTeleportEffectMessage(entity.Id,new Vector2(X,Y),AreaId );

                if (CenterOfBlock)
                {
                    message.Position += new Vector2(0.5f, 0.5f);
                }

                if(CanApply(message, entity, positionComponent, simulation))
                    ApplyLocally(message, entity, positionComponent, simulation);

            }
        }

        public static bool CanApply(OnTeleportEffectMessage effect, Entity e, TransformComponent transform, ISimulation simulation)
        {
            return true;
        }

        public static void ApplyLocally(OnTeleportEffectMessage effect, Entity e, TransformComponent transform, ISimulation simulation)
        {
            ApplyFromRemote(effect, e, transform, simulation);
            simulation.Entities.NoteForSend(effect);
        }

        public static void ApplyFromRemote(OnTeleportEffectMessage message, Entity e, TransformComponent transform, ISimulation simulation)
        {
            transform.CurrentAreaId = message.AreaId;
            transform.Position = message.Position;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(AreaId);
            bw.Write(CenterOfBlock);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            X = br.ReadInt32();
            Y = br.ReadInt32();
            AreaId = br.ReadInt32();
            CenterOfBlock = br.ReadBoolean();
        }

        public override void Serialize(ref byte[] target, ref int pos)
        {
            Serializer.WriteInt32(3, ref target, ref pos);
            Serializes<TeleportEffect>.Serialize(this, ref target, ref pos);
        }

        public static void InitializeSerialization()
        {
            Serializes<TeleportEffect>.Serialize = (TeleportEffect item, ref byte[] bytes, ref int index) => {
                Serializer.WriteInt32(item.X, ref bytes, ref index);
                Serializer.WriteInt32(item.Y, ref bytes, ref index);
                Serializer.WriteInt32(item.AreaId, ref bytes, ref index);
                Serializer.WriteBoolean(item.CenterOfBlock, ref bytes, ref index);
            };

            Serializes<TeleportEffect>.Deserialize =(byte[] bytes, ref int index) =>
                 new TeleportEffect(
                    Deserializer.ReadInt32(bytes, ref index),
                    Deserializer.ReadInt32(bytes, ref index),
                    Deserializer.ReadInt32(bytes, ref index),
                    Deserializer.ReadBoolean(bytes, ref index)
                );

            Deserializers[3] = (byte[] bytes, ref int index)
                => (EffectDefinition) Serializes<TeleportEffect>.Deserialize(bytes, ref index);
        }
    }
}