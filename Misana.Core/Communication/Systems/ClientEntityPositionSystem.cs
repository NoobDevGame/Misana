using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class ClientPositionSystem : BaseSystemR1<TransformComponent>
    {
        private INetworkClient _client;

        public ClientPositionSystem(INetworkClient client)
        {
            _client = client;
        }
    }
}