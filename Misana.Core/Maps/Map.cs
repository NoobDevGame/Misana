using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Systems;

namespace Misana.Core.Maps
{
    public class Map
    {
        public Area StartArea { get; set; }
        public string Name { get; set; }
        public List<Area> Areas { get; set; }

        public static readonly Version MapVersion = new Version(0,2);

        public Dictionary<string,EntityDefinition> GlobalEntityDefinitions { get; set; } = new Dictionary<string, EntityDefinition>();

        private int entityIndex = 0;

        public Map(string name, Area startArea, List<Area> areas, int index)
        {
            if (startArea == null)
                throw new ArgumentNullException(nameof(startArea));

            if (areas == null)
                throw  new ArgumentNullException(nameof(areas));

            if (areas.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(areas),"area must be greater the 0.");

            Name = name;
            StartArea = startArea;
            Areas = areas;
            entityIndex = index;
        }

        private Map() {}

        public Area GetAreaById(int areaId)
        {
            return Areas.First(i => i.Id == areaId);
        }

        public int GetNextDefinitionId()
        {
            return Interlocked.Increment(ref entityIndex);
        }
    }
}