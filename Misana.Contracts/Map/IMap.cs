using System.Collections.Generic;
using Misana.Contracts.Entity;

namespace Misana.Contracts.Map
{
    public interface IMap
    {
        IArea StartArea { get; }

        string Name { get; }

        IArea[] Areas { get; }

        void SetId(IEntity entity);

        void Load();
        IEntity GetEntityById(int id);
    }
}
