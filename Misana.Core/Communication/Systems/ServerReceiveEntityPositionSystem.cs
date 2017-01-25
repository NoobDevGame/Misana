using System.Linq;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class ServerReceiveEntityPositionSystem : BaseSystemR1<TransformComponent>
    {
        private readonly INetworkReceiver client;

        private readonly IBroadcastSender sender;

        public ServerReceiveEntityPositionSystem(INetworkReceiver client,IBroadcastSender sender)
        {
            this.client = client;
            this.sender = sender;
        }

        public override void Tick()
        {
            EntityPositionMessage message;
            INetworkClient senderClient;
            while (client.TryGetMessage(out message,out senderClient))
            {
                var component = Entities.First(i => i.Id == message.entityId).Get<TransformComponent>();
                component.Position = message.position;
                sender.SendMessage(ref message,senderClient.ClientId);
            }
        }
    }
}