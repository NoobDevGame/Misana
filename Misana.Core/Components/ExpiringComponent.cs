using System;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class ExpiringComponent : Component<ExpiringComponent>
    {
        public override void CopyTo(ExpiringComponent other)
        {
            other.TimeLeft = TimeLeft;
        }

        [Copy, Reset]
        public TimeSpan TimeLeft;
    }
}