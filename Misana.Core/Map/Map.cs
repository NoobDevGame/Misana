using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Misana.Core.Ecs;

namespace Misana.Core.Map
{
    public class Map
    {
        public Area StartArea { get; }
        public string Name { get; }
        public Area[] Areas { get; }

        public EntityManager Entities { get; }

        public static readonly Version MapVersion = new Version(0,1);


        public Map(string name, Area startArea, Area[] areas)
        {
            if(startArea == null)
                throw  new ArgumentNullException(nameof(startArea));

            if (areas == null)
                throw  new ArgumentNullException(nameof(areas));

            if (areas.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(areas),"area must be greater the 0.");

            Name = name;
            StartArea = startArea;
            Areas = areas;

            Entities = EntityManager.Create(name, new List<Assembly> {typeof(Entity).Assembly });
        }

        private Map() {}
    }
}