using System;
using System.Collections.Generic;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Effects;

namespace Misana.Core.Events.OnUse
{
    public class ApplyEffectOnUseEvent : OnUseEvent
    {
        private EffectDefinition _eff;

        public ApplyEffectOnUseEvent()
        {

        }

        public ApplyEffectOnUseEvent(EffectDefinition eff)
        {
            _eff = eff;
        }

        protected override bool ApplyToTarget(EntityManager manager, Entity self, Vector2 target, ISimulation simulation)
        {
            _eff?.Apply(self, simulation);
            return true;
        }

        public override OnUseEvent Copy()
        {
            return new ApplyEffectOnUseEvent(_eff) { CoolDown = CoolDown };
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write((byte)RunsOn);
            bw.Write(_eff.GetType().AssemblyQualifiedName);
            _eff.Serialize(version,bw);
            bw.Write(CoolDown.TotalMilliseconds);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            RunsOn = (RunsOn)br.ReadByte();
            var name = br.ReadString();
            var type = Type.GetType(name);
            _eff = (EffectDefinition) Activator.CreateInstance(type);
            _eff.Deserialize(version,br);
            CoolDown = TimeSpan.FromMilliseconds(br.ReadDouble());
        }
    }
}