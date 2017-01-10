using System.Data;
using Misana.Contracts.Entity;
using Misana.Contracts.Map;

namespace Misana.Core.Map
{
    public class Map :IMap
    {
        public IArea StartArea { get; }
        public string Name { get; }
        public IArea[] Areas { get; }

        public Map(string name, IArea startArea, IArea[] areas)
        {
            Name = name;
            StartArea = startArea;
            Areas = areas;
        }

        public void SetId(IEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Load()
        {
            throw new System.NotImplementedException();
        }

        public IEntity GetEntityById(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}