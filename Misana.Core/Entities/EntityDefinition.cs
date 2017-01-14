using System.Collections.Generic;

namespace Misana.Core.Entities
{
    public class EntityDefinition
    {
        public string Name { get; set; }
        public List<ComponentDefinition> Definitions { get; } = new List<ComponentDefinition>();

        public EntityDefinition(string name)
        {
            Name = name;
        }

        public EntityDefinition()
        {

        }
    }
}