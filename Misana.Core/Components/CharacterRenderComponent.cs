using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class CharacterRenderComponent : Component<CharacterRenderComponent>
    {
        public Index2 TilePosition { get; set; }

        public override void CopyTo(CharacterRenderComponent other)
        {
            other.TilePosition = TilePosition;
        }
    }
}
