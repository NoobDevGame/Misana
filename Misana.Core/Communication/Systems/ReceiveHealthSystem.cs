using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class ReceiveHealthSystem : BaseSystemR1<HealthComponent>
    {
        private INetworkReceiver _client;

        public ReceiveHealthSystem(INetworkReceiver client)
        {
            _client = client;
        }

        public override void Tick()
        {
            EntityHealthMessage message;
            while (_client.TryGetMessage(out message))
            {

                var entity = Manager.GetEntityById(message.EntityId);
                if (entity != null)
                {
                    var transformCmp = entity.Get<HealthComponent>();
                    transformCmp.Current = message.Health;
                }
            }
        }
    }
}