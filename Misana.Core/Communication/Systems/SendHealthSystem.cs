using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Network;

namespace Misana.Core.Communication.Systems
{
    public class SendHealthSystem : BaseSystemR1N1<HealthComponent,OnLocalSimulationComponent>
    {

        public SendHealthSystem()
        {
        }

        protected override void Update(Entity e, HealthComponent r1)
        {
            EntityHealthMessage message = new EntityHealthMessage(e.Id,r1.Current);
            Manager.NoteForSend(message);
        }
    }
}