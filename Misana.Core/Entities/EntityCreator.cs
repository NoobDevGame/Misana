using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities
{
    public static class EntityCreator
    {
        public static int CreateEntity(EntityManager manager,Map map,EntityDefinition definition)
        {
            var entity = manager.NewEntity();

            return CreateEntity(definition, map,entity);
        }

        public static int CreateEntity(EntityDefinition definition,Map map, Entity entity)
        {
            foreach (var componentDefinition in definition.Definitions)
            {
                componentDefinition.ApplyDefinition(entity, map);
            }

            entity.Commit();

            return entity.Id;
        }
    }
}