using System.Linq;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class ReceiveEntityPositionSystem : BaseSystemR1<TransformComponent>
    {
        private INetworkReceiver _client;

        public ReceiveEntityPositionSystem(INetworkReceiver client)
        {
            _client = client;
        }

        public override void Tick()
        {
            EntityPositionMessage message;
            while (_client.TryGetMessage(out message))
            {

                var entity = Manager.GetEntityById(message.entityId);
                if (entity != null)
                {
                    var transformCmp = entity.Get<TransformComponent>();
                    transformCmp.Position = message.position;

                    var facingCmp = entity.Get<FacingComponent>();
                    if (facingCmp != null)
                        facingCmp.Facing = message.Facing;

                }
            }
        }
	}
}