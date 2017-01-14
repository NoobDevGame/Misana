using Misana.Core.Ecs;

namespace Misana.Core.Entities
{
    public static class EntityCreator
    {
        public static int CreateEntity(EntityManager manager,EntityDefinition definition)
        {
            var entity = manager.NewEntity();

            return CreateEntity(definition,entity);
        }

        public static int CreateEntity(EntityDefinition definition, Entity entity)
        {
            foreach (var componentDefinition in definition.Definitions)
            {
                componentDefinition.ApplyDefinition(entity);
            }

            entity.Commit();

            return entity.Id;
        }
    }
}