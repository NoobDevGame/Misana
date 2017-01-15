using System;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Effects;

namespace Misana.Core.Events.Collision
{
    public class ApplyEffectOnCollisionEvent : OnCollisionEvent
    {
        public EffectDefinition Effect;
        public EffectCondition Condition;
        public ApplyEffectOnCollisionEvent(EffectDefinition deff, EffectCondition condition = null)
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
            var effect = (EffectDefinition)Activator.CreateInstance(Type.GetType(typeName2));
            effect.Deserialize(version, br);
        }

        protected override bool CanApply(EntityManager manager, Entity target, World world)
        {
            return Condition?.Test(target, world) ?? true;
        }

        protected override bool ApplyToEntity(EntityManager manager, Entity target, World world)
        {
            Effect.Apply(target, world);
            return true;
        }
    }
}