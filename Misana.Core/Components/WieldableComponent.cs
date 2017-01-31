using System.Collections.Generic;
using System.Linq;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;
using Misana.Core.Events.OnUse;

namespace Misana.Core.Components
{
    public class WieldableComponent : Component<WieldableComponent>
    {
        [Copy, Reset]
        public List<OnUseEvent> OnUseEvents = new List<OnUseEvent>();

        public override void CopyTo(WieldableComponent other)
        {
            other.OnUseEvents = new List<OnUseEvent>(OnUseEvents.Select(e => e.Copy()));
        }
    }
}