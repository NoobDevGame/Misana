using System.Linq;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class ReceiveEntityPositionSystem : BaseSystemR1<TransformComponent>
    {
        private INetworkClient _client;

        public ReceiveEntityPositionSystem(INetworkClient client)
        {
            _client = client;
        }

        public override void Tick()
        {
            EntityPositionMessage? message;
            while (_client.TryGetMessage(out message))
            {
                var component = Entities.First(i => i.Id == message.Value.entityId).Get<TransformComponent>();
                component.Position = message.Value.position;
            }
        }
	}
}