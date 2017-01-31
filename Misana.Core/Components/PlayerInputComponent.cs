using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class PlayerInputComponent : Component<PlayerInputComponent>
    {
        [Copy, Reset]
        public Vector2 Move;

        [Copy, Reset]
        public Vector2 Facing;

        [Copy, Reset]
        public bool Attacking;

        [Copy, Reset]
        public bool Interact;

        [Copy, Reset]
        public bool Drop;

        [Copy, Reset]
        public bool PickUp;

        [Copy, Reset]
        public Vector2 MousePosition;
    }
}
