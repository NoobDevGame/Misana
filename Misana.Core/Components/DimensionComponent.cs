using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class DimensionComponent : Component<DimensionComponent>
    {
        public float Radius { get; set; }

        public Vector2 Size => new Vector2(2 * Radius, 2 * Radius);
        public Vector2 HalfSize => new Vector2( Radius, Radius);
    }
}
