using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Entities.BaseDefinition
{
    public class EntityColliderDefinition : ComponentDefinition<EntityColliderComponent>
    {
        public bool Blocked { get; set; } = true;
        public bool Fixed { get; set; } = false;

        public float Mass { get; set; } = 50f;

        public override void OnApplyDefinition(Entity entity, EntityColliderComponent component)
        {
            component.Blocked = Blocked;
            component.Fixed = Fixed;
            component.Mass = Mass;
        }
    }
}