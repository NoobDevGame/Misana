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
    }
}