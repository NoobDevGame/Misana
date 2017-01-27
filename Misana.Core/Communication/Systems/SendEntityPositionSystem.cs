using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class SendEntityPositionSystem : BaseSystemR2O1<TransformComponent,SendComponent,FacingComponent>
    {
        private INetworkSender _client;

        public SendEntityPositionSystem(INetworkSender client)
        {
            _client = client;
        }

        protected override void Update(Entity e, TransformComponent r1,SendComponent r2,FacingComponent o1)
        {
            EntityPositionMessage message = new EntityPositionMessage(e.Id,r1);
            message.Facing = o1?.Facing ?? Vector2.Zero;


            _client.SendMessage(ref message);
        }
    }
}