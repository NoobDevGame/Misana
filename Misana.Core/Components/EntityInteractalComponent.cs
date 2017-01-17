using System.Collections.Generic;
using Misana.Core.Ecs;
using Misana.Core.Events.Entities;

namespace Misana.Core.Components
{
    public class EntityInteractableComponent : Component<EntityInteractableComponent>
    {
        public float InteractionRadius = 0.6f;
        public bool Interacting;
        public List<OnEvent> OnInteractionEvents = new List<OnEvent>(2);

        public override void CopyTo(EntityInteractableComponent other)
        {
            other.Interacting = Interacting;
            other.InteractionRadius = InteractionRadius;
            other.OnInteractionEvents = new List<OnEvent>(OnInteractionEvents);
        }
    }
}