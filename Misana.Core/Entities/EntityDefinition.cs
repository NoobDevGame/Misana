using System.Collections.Generic;
using System.Threading;

namespace Misana.Core.Entities
{
    public class EntityDefinition
    {
        public string Name { get; set; }
        public List<ComponentDefinition> Definitions { get; } = new List<ComponentDefinition>();

        private static int globalIndex = 0;

        public int Id;

        public EntityDefinition(string name,int id)
        {
            Name = name;
            Id = id;
            if (id > Id)
                Id = id;
        }

        public EntityDefinition(int id)
        {
            Id = id;
            if (id > Id)
                Id = id;
        }

        public EntityDefinition(string name)
        {
            Name = name;
            Id = Interlocked.Increment(ref Id);
        }

        public EntityDefinition()
        {
            Id = Interlocked.Increment(ref Id);
        }
    }
}