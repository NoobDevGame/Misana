using Misana.Core.Communication.Messages;

namespace Misana.Core.Client
{
    public interface IClientRpcMessageHandler
    {
        void Handle(StartSimulationMessageResponse message);
        void Handle(OnStartSimulationMessage message);
        void Handle(StartSimulationMessageRequest message);
        void Handle(LoginMessageRequest message);
        void Handle(LoginMessageResponse message);
        void Handle(JoinWorldMessageRequest message);
        void Handle(JoinWorldMessageResponse message);
        void Handle(OnJoinWorldMessage message);
        void Handle(PlayerInfoMessage message);
        void Handle(GetOtherPlayersMessageRequest message);
        void Handle(CreateWorldMessageResponse message);
        void Handle(CreateWorldMessageRequest message);
        void Handle(CreateEntityMessageRequest message);
        void Handle(CreateEntityMessageResponse message);
        void Handle(ChangeMapMessageRequest message);
        void Handle(ChangeMapMessageResponse message);
        void Handle(ReadWorldsMessageRequest message);
        void Handle(WorldInformationMessage message);
    }
}