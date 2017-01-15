using System.Collections.Generic;
using Misana.Core.Ecs;
using Misana.Core.Events.Collision;

namespace Misana.Core.Components
{
    public class EntityInteractableComponent : Component<EntityInteractableComponent>
    {
        public float InteractionRadius = 0.6f;
        public bool Interacting;
        public List<OnCollisionEvent> OnInteractionEvents = new List<OnCollisionEvent>(2);

        public override void CopyTo(EntityInteractableComponent other)
        {
            other.Interacting = Interacting;
            other.InteractionRadius = InteractionRadius;
            other.OnInteractionEvents = new List<OnCollisionEvent>(OnInteractionEvents);
        }
    }
}