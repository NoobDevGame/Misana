using Misana.Core.Entities;
using Misana.Core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Models
{
    public class MapModel
    {
        public Area StartArea { get; set; }
        public string Name { get; set; }
        public List<Area> Areas { get; set; }

        public static readonly Version MapVersion = new Version(0, 2);

        public Dictionary<string, EntityDefinition> GlobalEntityDefinitions { get; set; } = new Dictionary<string, EntityDefinition>();

        public MapModel(string name)
        {
            Name = name;
            Areas = new List<Area>();
        }

        public MapModel(Map map)
        {
            StartArea = map.StartArea;
            Name = map.Name;
            Areas = map.Areas;
            GlobalEntityDefinitions = map.GlobalEntityDefinitions;
        }

        public Map ToMap()
        {
            return new Map(Name, StartArea, Areas,0) { GlobalEntityDefinitions = GlobalEntityDefinitions };
        }

        private MapModel() { }

        public Area GetAreaById(int areaId)
        {
            return Areas.First(i => i.Id == areaId);
        }
    }
}
