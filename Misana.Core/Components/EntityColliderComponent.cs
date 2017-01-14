using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class EntityColliderComponent : Component<EntityColliderComponent>
    {
        public float Mass { get; set; }
        public bool Fixed { get; set; }

        public bool AppliesSideEffect { get; set; }
        public bool Blocked { get; set; }

        public EntityColliderComponent()
        {
            Mass = 50f;
        }
    }
}
