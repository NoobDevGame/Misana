using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities
{
    public static class EntityCreator
    {
        public static EntityBuilder CreateEntity(EntityManager manager,Map map,EntityDefinition definition)
        {
            return CreateEntity(definition, map, new EntityBuilder());
        }

        public static EntityBuilder CreateEntity(EntityDefinition definition,Map map, EntityBuilder entity)
        {
            foreach (var componentDefinition in definition.Definitions)
            {
                componentDefinition.ApplyDefinition(entity, map);
            }

            return entity;
        }
    }
}