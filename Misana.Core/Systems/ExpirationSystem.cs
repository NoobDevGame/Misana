using System;
using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class ExpirationSystem : BaseSystemR1<ExpiringComponent>
    {
        protected override void Update(Entity e, ExpiringComponent r1)
        {
            r1.TimeLeft -= GameTime.ElapsedTime;

            if (r1.TimeLeft <= TimeSpan.Zero)
                Manager.RemoveEntity(e.Id);
        }
    }
}