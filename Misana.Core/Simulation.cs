using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core
{
    public class Simulation
    {

        public World World { get; private set; }
        public Simulation(World world)
        {
            World = world;
        }
    }
}
