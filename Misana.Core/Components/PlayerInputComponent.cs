using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class PlayerInputComponent : Component<PlayerInputComponent>
    {
        public Vector2 Move;
        public bool Interact;

        public Vector2 MousePosition;

        public override void CopyTo(PlayerInputComponent other)
        {
            other.Move = Move;
        }
    }
}
