using System;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components.StatusComponent
{
    public class TimeDamageComponent : Component<TimeDamageComponent>
    {
        public override void CopyTo(TimeDamageComponent other)
        {
            other.EffectTime = EffectTime;
            other.DamagePerSeconds = DamagePerSeconds;
            other.CurrentTime = CurrentTime;
        }

        [Copy, Reset]
        public float DamagePerSeconds;
        [Copy, Reset]
        public TimeSpan EffectTime;
        [Copy, Reset]
        public TimeSpan CurrentTime;
    }
}