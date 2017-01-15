using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Systems;

namespace Misana.Core.Maps
{
    public class Map
    {
        public Area StartArea { get; }
        public string Name { get; }
        public Area[] Areas { get; }

        public static readonly Version MapVersion = new Version(0,1);

        public Dictionary<string,EntityDefinition> GlobalEntityDefinitions { get; set; } = new Dictionary<string, EntityDefinition>();

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
        }

        private Map() {}

        public Area GetAreaById(int areaId)
        {
            return Areas.First(i => i.Id == areaId);
        }
    }
}