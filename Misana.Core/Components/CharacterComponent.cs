﻿using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class CharacterComponent : Component<CharacterComponent>
    {
        [Copy, Reset]
        public string Name;
    }
}
