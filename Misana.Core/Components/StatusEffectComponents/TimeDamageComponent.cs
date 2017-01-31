using System;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components.StatusEffectComponents
{
    public class TimeDamageComponent : Component<TimeDamageComponent>
    {
        [Copy, Reset]
        public float DamagePerSeconds;
        [Copy, Reset]
        public TimeSpan EffectTime;
        [Copy, Reset]
        public TimeSpan CurrentTime;
    }
}