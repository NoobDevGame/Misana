using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class EntityCollider : Component<EntityCollider>
    {
        public float Mass { get; set; }

        public bool Fixed { get; set; }

        public EntityCollider()
        {
            Mass = 50f;
        }
    }
}
