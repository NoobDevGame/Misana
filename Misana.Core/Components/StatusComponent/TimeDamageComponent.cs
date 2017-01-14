using System;
using Misana.Core.Ecs;

namespace Misana.Core.Components.StatusComponent
{
    public class TimeDamageComponent : Component<TimeDamageComponent>
    {
        public override void Reset()
        {
            CurrentTime = TimeSpan.Zero;
        }

        public override void CopyTo(TimeDamageComponent other)
        {
            other.EffectTime = EffectTime;
            other.DamagePerSeconds = DamagePerSeconds;
            other.CurrentTime = CurrentTime;
        }

        public float DamagePerSeconds;
        public TimeSpan EffectTime;
        public TimeSpan CurrentTime;
    }
}