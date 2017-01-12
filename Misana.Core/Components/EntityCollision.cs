using System.Collections.Generic;
using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class EntityCollision : Component<EntityCollision>
    {
        public List<int> OtherEntityIds = new List<int>();
    }
}