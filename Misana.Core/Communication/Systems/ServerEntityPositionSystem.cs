using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Network;

namespace Misana.Core.Communication.Systems
{
    public class ServerEntityPositionSystem : BaseSystemR1<TransformComponent>
    {
        private INetworkClient _client;

        public ServerEntityPositionSystem(INetworkClient client)
        {
            _client = client;
        }
    }
}