using System.Collections.Generic;
using System.Linq;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;
using Misana.Core.Events.Entities;

namespace Misana.Core.Components
{
    public class CreateComponent : Component<CreateComponent>
    {
        [Copy, Reset]
        public List<OnEvent> OnCreateEvent = new List<OnEvent>(2);
    }
}