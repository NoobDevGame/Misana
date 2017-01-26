﻿using System;
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

        protected override bool CanApply(EntityManager manager, Entity target, ISimulation simulation)
        {
            return Condition?.Test(target, simulation) ?? true;
        }

        internal override async Task<bool> ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, ISimulation simulation)
        {
            Effect.Apply(target, simulation);
            return await Task.FromResult(true);
        }

        public override OnEvent Copy()
        {
            return new ApplyEffectEvent(Effect, Condition);
        }
    }
}