using System;
using System.IO;
using System.Threading.Tasks;
using Misana.Core.Ecs;
using Misana.Core.Effects;

namespace Misana.Core.Events.Entities
{
    public class ApplyEffectEvent : OnEvent
    {
        public EffectDefinition Effect;
        public EffectCondition Condition;

        public ApplyEffectEvent()
        {

        }

        public ApplyEffectEvent(EffectDefinition deff, EffectCondition condition = null)
        {
            Effect = deff;
            Condition = condition;
        }


        public override void Serialize(Version version, BinaryWriter bw)
        {
            if (Condition != null)
            {
                bw.Write(true);
                bw.Write(Condition.GetType().AssemblyQualifiedName);
                Condition.Serialize(version, bw);
            }
            else
            {
                bw.Write(false);
            }

            bw.Write(Effect.GetType().AssemblyQualifiedName);
            Effect.Serialize(version, bw);

            bw.Write((byte)ApplyTo);
            bw.Write(CoolDown.TotalMilliseconds);
            bw.Write(Debounce.TotalMilliseconds);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            var conditionExist = br.ReadBoolean();
            if (conditionExist)
            {
                var typeName = br.ReadString();
                var condition = (EffectCondition)Activator.CreateInstance(Type.GetType(typeName));
                condition.Deserialize(version, br);
                Condition = condition;
            }

            var typeName2 = br.ReadString();
            Effect = (EffectDefinition)Activator.CreateInstance(Type.GetType(typeName2));
            Effect.Deserialize(version, br);

            ApplyTo = (ApplicableTo) br.ReadByte();
            CoolDown = TimeSpan.FromMilliseconds(br.ReadDouble());
            Debounce = TimeSpan.FromMilliseconds(br.ReadDouble());
        }

        protected override bool CanApply(EntityManager manager, Entity target, ISimulation simulation)
        {
            return Condition?.Test(target, simulation) ?? true;
        }

        internal override bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, ISimulation simulation)
        {
            Effect.Apply(target, simulation);
            return true;
        }

        public override OnEvent Copy()
        {
            return new ApplyEffectEvent(Effect, Condition);
        }
    }
}