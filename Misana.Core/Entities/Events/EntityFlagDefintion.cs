using Misana.Core.Components.Events;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.Events
{
    public class EntityFlagDefintion : ComponentDefinition<EntityFlagComponent>
    {
        public override void OnApplyDefinition(EntityBuilder entity, Map map, EntityFlagComponent component, ISimulation sim)
        {
        }
    }
}