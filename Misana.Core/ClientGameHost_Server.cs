using System.Threading;
using Misana.Core.Communication.Messages;
using Misana.Network;

namespace Misana.Core
{
    public partial class ClientGameHost
    {
        protected override void OnCreateWorld(CreateWorldMessageRequest message,MessageHeader header,NetworkClient client)
        {
            CreateWorldMessageResponse messageResponse = new CreateWorldMessageResponse(false,0,0);
            client.SendMessage(ref messageResponse,header.MessageId);
        }
    }
}