using System.Collections.Generic;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;
using Misana.Core.Events.Entities;

namespace Misana.Core.Components
{
    public class EntityInteractableComponent : Component<EntityInteractableComponent>
    {
        [Copy, Reset(0.6f)]
        public float InteractionRadius = 0.6f;

        [Copy, Reset]
        public bool Interacting;

        [Copy, Reset]
        public List<OnEvent> OnInteractionEvents = new List<OnEvent>(2);

        public override void CopyTo(EntityInteractableComponent other)
        {
            other.Interacting = Interacting;
            other.InteractionRadius = InteractionRadius;
            other.OnInteractionEvents = new List<OnEvent>(OnInteractionEvents);
        }
    }
}