using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class SpriteInfoComponent : Component<SpriteInfoComponent>
    {
        [Copy, Reset] public Index2 TilePosition;

        public override void CopyTo(SpriteInfoComponent other)
        {
            other.TilePosition = TilePosition;
        }
    }
}
