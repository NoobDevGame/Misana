using System;
using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class CollisionApplicator : Component<CollisionApplicator>
    {
        public Action<Entity,World> Action { get; set; }
    }
}