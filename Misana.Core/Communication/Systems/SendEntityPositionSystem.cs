﻿using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class SendEntityPositionSystem : BaseSystemR2<TransformComponent,SendComponent>
    {
        private INetworkSender _client;

        public SendEntityPositionSystem(INetworkSender client)
        {
            _client = client;
        }

        protected override void Update(Entity e, TransformComponent r1,SendComponent r2)
        {
            EntityPositionMessage message = new EntityPositionMessage(e.Id,r1);
            _client.SendMessage(ref message);
        }
    }
}