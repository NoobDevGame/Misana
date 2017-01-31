using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities
{
    public static class EntityCreator
    {
        public static EntityBuilder CreateEntity(EntityDefinition definition,Map map, EntityBuilder entity, ISimulation sim)
        {
            foreach (var componentDefinition in definition.Definitions)
            {
                componentDefinition.ApplyDefinition(entity, map, sim);
            }

            return entity;
        }
    }
}