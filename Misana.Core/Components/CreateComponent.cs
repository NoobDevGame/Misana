using System.Collections.Generic;
using System.Linq;
using Misana.Core.Ecs;
using Misana.Core.Events.Entities;

namespace Misana.Core.Components
{
    public class CreateComponent : Component<CreateComponent>
    {
        public List<OnEvent> OnCreateEvent = new List<OnEvent>(2);

        public override void CopyTo(CreateComponent other)
        {
            other.OnCreateEvent = new List<OnEvent>(OnCreateEvent.Select(e => e.Copy())); //TODO: :(
        }


    }
}