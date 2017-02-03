using System;
using System.IO;
using Misana.Core.Communication.Components;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Core.Events;

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
            var area = simulation.CurrentMap.GetAreaById( message.AreaId);
            transform.CurrentArea = area;
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
    }
}