using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class EntityCollision : Component<EntityCollision>
    {
        public int OtherEntityId;
    }
}