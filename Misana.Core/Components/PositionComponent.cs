using Misana.Core.Ecs;
using Misana.Core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class PositionComponent : Component<PositionComponent>
    {
        public Vector2 Position { get; set; }
        public Area CurrentArea { get; set; }
    }
}
