using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class SendHealthSystem : BaseSystemR1N1<HealthComponent,OnLocalSimulationComponent>
    {
        private INetworkSender _client;

        public SendHealthSystem(INetworkSender client)
        {
            _client = client;
        }

        protected override void Update(Entity e, HealthComponent r1)
        {
            EntityHealthMessage message = new EntityHealthMessage(e.Id,r1.Current);
            _client.SendMessage(ref message);
        }
    }
}