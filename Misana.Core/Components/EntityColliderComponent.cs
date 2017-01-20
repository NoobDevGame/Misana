using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Ecs.Changes;
using Misana.Core.Events.Entities;

namespace Misana.Core.Components
{
    public class EntityColliderComponent : Component<EntityColliderComponent>
    {
        public override void Reset()
        {
            base.Reset();
            Mass = 50f;
            Fixed = false;
            Blocked = false;
            OnCollisionEvents.Clear();
        }

        public float Mass = 50f;
        public bool Fixed;

        public List<OnEvent> OnCollisionEvents = new List<OnEvent>(2);
        public bool Blocked;

        public override void CopyTo(EntityColliderComponent other)
        {
            other.Mass = Mass;
            other.Fixed = Fixed;
            other.Blocked = Blocked;
            other.OnCollisionEvents = new List<OnEvent>(OnCollisionEvents.Select(e => e.Copy())); //TODO: :(
        }
    }

    
}
