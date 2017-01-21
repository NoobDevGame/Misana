using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class SendEntityPositionSystem : BaseSystemR1<TransformComponent>
    {
        private INetworkClient _client;

        public SendEntityPositionSystem(INetworkClient client)
        {
            _client = client;
        }

        protected override void Update(Entity e, TransformComponent r1)
        {
            EntityPositionMessage message = new EntityPositionMessage(e.Id,r1);
            _client.SendMessage(ref message);
        }
    }
}