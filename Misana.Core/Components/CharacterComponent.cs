using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class CharacterComponent : Component<CharacterComponent>
    {
        public string Name { get; set; }
    }
}
