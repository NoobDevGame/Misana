using Misana.Core.Communication.Messages;
using Misana.Core.Network;

namespace Misana.Core.Server
{
    public interface IServerRpcMessageHandler
    {
        void Handle(StartSimulationMessageResponse message, byte messageId, IClientOnServer client);
        void Handle(OnStartSimulationMessage message, byte messageId, IClientOnServer client);
        void Handle(StartSimulationMessageRequest message, byte messageId, IClientOnServer client);
        void Handle(LoginMessageRequest message,  byte messageId,IClientOnServer client);
        void Handle(LoginMessageResponse message,  byte messageId,IClientOnServer client);
        void Handle(JoinWorldMessageRequest message, byte messageId, IClientOnServer client);
        void Handle(JoinWorldMessageResponse message, byte messageId, IClientOnServer client);
        void Handle(OnJoinWorldMessage message, byte messageId, IClientOnServer client);
        void Handle(PlayerInfoMessage message,  byte messageId,IClientOnServer client);
        void Handle(GetOtherPlayersMessageRequest message, byte messageId, IClientOnServer client);
        void Handle(CreateWorldMessageResponse message, byte messageId, IClientOnServer client);
        void Handle(CreateWorldMessageRequest message,  byte messageId,IClientOnServer client);
        void Handle(ChangeMapMessageRequest message,  byte messageId,IClientOnServer client);
        void Handle(ChangeMapMessageResponse message, byte messageId, IClientOnServer client);
        void Handle(ReadWorldsMessageRequest message, byte messageId, IClientOnServer client);
        void Handle(WorldInformationMessage message, byte messageId, IClientOnServer client);
        void Handle(InitialGameState message, byte messageId, IClientOnServer client);
        void Handle(HotJoinedMessage message, byte messageId, IClientOnServer client);
        void Handle(PlayerLeftMessage message, byte messageId, IClientOnServer client);
    }
}