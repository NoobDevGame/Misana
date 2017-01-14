using System.Collections.Generic;
using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class EntityCollisionComponent : Component<EntityCollisionComponent>
    {
        public List<int> OtherEntityIds = new List<int>();
    }
}