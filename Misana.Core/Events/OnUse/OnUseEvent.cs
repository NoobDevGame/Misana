using System;
using Misana.Core.Ecs;
using Misana.Serialization;

namespace Misana.Core.Events.OnUse
{
    public abstract class OnUseEvent : EventDefinition
    {
        public TimeSpan CoolDown;
        protected TimeSpan LastExecution;

        protected virtual bool CanApply(EntityManager manager, Entity target, ISimulation simulation)
        {
            return true;
        }

        private readonly object _lockObj = new object();


        public virtual void Apply(EntityManager manager, Entity self, Vector2 target, ISimulation simulation)
        {
            if (RunsOn != RunsOn.Both && (byte)manager.Mode != (byte)RunsOn)
                return;

            if (LastExecution != TimeSpan.Zero && CoolDown != TimeSpan.Zero)
            {
                if (manager.GameTime.TotalTime - LastExecution < CoolDown)
                    return;
            }

            var applied = ApplyToTarget(manager, self, target, simulation);

            if(applied)
                LastExecution = manager.GameTime.TotalTime;
        }

        public abstract OnUseEvent Copy();

        protected abstract bool ApplyToTarget(EntityManager manager, Entity self, Vector2 target, ISimulation simulation);

        public abstract void Serialize(ref byte[] bytes, ref int index);

        public static Deserialize<OnUseEvent>[] Deserializers;

        public static void Initialize() {
            Deserializers = new Deserialize<OnUseEvent>[1];

            ApplyEffectOnUseEvent.InitializeSerialization();

            Serializes<OnUseEvent>.Deserialize = (byte[] bytes, ref int index) => {
                var idx = Deserializer.ReadInt32(bytes, ref index);
                return Deserializers[idx](bytes, ref index);
            };

            Serializes<OnUseEvent>.Serialize = (OnUseEvent item, ref byte[] bytes, ref int index) => {
                item.Serialize(ref bytes, ref index);
            };
        }
    }
}