using System;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class ExpiringComponent : Component<ExpiringComponent>
    {
        [Copy, Reset]
        public TimeSpan TimeLeft;
    }
}