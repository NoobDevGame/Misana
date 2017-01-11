using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Maps;

namespace Misana
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MisanaGame game = new MisanaGame())
            {
                game.Run(60, 60);
            }
        }
    }
}
