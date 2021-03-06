﻿using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class WieldingComponent : Component<WieldingComponent>
    {
        [Copy, Reset]
        public bool TwoHanded;

        [Copy, Reset]
        public int LeftHandEntityId;

        [Copy, Reset]
        public int RightHandEntityId;

        [Copy, Reset]
        public bool Use;
    }
}