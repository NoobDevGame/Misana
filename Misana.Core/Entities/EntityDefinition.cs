using System.Collections.Generic;
using System.Threading;

namespace Misana.Core.Entities
{
    public class EntityDefinition
    {
        public string Name { get; set; }
        public List<ComponentDefinition> Definitions { get; } = new List<ComponentDefinition>();

        public int Id;

        public EntityDefinition(string name,int id)
        {
            Name = name;
        }

        public EntityDefinition(int id)
        {
            Id = id;
        }
    }
}