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

        public float DamagePerSeconds { get; set; }
        public TimeSpan EffectTime { get; set; }
        public TimeSpan CurrentTime { get; set; }
    }
}