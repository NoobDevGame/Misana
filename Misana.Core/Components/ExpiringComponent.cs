using System;
using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class ExpiringComponent : Component<ExpiringComponent>
    {
        public override void CopyTo(ExpiringComponent other)
        {
            other.TimeLeft = TimeLeft;
        }

        public TimeSpan TimeLeft;
    }
}