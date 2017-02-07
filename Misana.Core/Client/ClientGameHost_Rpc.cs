using System;
using System.Collections.Generic;
using Misana.Core.Communication;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Core.Maps;
using Misana.Core.Network;

namespace Misana.Core.Client
{
    public partial class ClientGameHost : IClientRpcMessageHandler
    {
        public event Action<WorldInformation> WorldInfoReceived;
        public event Action<PlayerInfo> PlayerInfoReceived;
        public event Action<int> PlayerLeft;
        public event Action SimulationStarted;
        public event Action<InitialGameState> InitialGameStateReceived;

        void IClientRpcMessageHandler.Handle(StartSimulationMessageResponse message)
        {
            ClientMessageHelper<StartSimulationMessageResponse>.LastResult = message;
            ClientMessageHelper<StartSimulationMessageResponse>.Semaphore?.Release();
        }

        void IClientRpcMessageHandler.Handle(OnStartSimulationMessage message)
        {
            SimulationStarted?.Invoke();
        }

        void IClientRpcMessageHandler.Handle(LoginMessageResponse message)
        {
            ClientMessageHelper<LoginMessageResponse>.LastResult = message;
            ClientMessageHelper<LoginMessageResponse>.Semaphore?.Release();
        }

        void IClientRpcMessageHandler.Handle(JoinWorldMessageResponse message)
        {
            ClientMessageHelper<JoinWorldMessageResponse>.LastResult = message;
            ClientMessageHelper<JoinWorldMessageResponse>.Semaphore?.Release();
        }

        void IClientRpcMessageHandler.Handle(OnJoinWorldMessage message)
        {
            PlayerInfoReceived?.Invoke(new PlayerInfo(message.Name, message.PlayerId));
        }

        void IClientRpcMessageHandler.Handle(PlayerInfoMessage message)
        {
            PlayerInfoReceived?.Invoke(new PlayerInfo(message.Name, message.PlayerId));
        }

        void IClientRpcMessageHandler.Handle(CreateWorldMessageResponse message)
        {
            ClientMessageHelper<CreateWorldMessageResponse>.LastResult = message;
            ClientMessageHelper<CreateWorldMessageResponse>.Semaphore?.Release();
        }

        void IClientRpcMessageHandler.Handle(ChangeMapMessageResponse message)
        {
            ClientMessageHelper<ChangeMapMessageResponse>.LastResult = message;
            ClientMessageHelper<ChangeMapMessageResponse>.Semaphore?.Release();
        }

        public void Handle(WorldInformationMessage message)
        {
            WorldInfoReceived?.Invoke(new WorldInformation(message));
        }

        public void Handle(InitialGameState message)
        {
            InitialGameStateReceived?.Invoke(message);
        }

        public void Handle(HotJoinedMessage message)
        {
            PlayerInfoReceived?.Invoke(new PlayerInfo(message.Name, message.PlayerId));
            foreach (var e in message.Entities)
            {
                e.Manager = Entities;

                var eSend = e.Get<SendComponent>();
                if (eSend != null)
                {
                    ComponentRegistry<SendComponent>.Release(eSend);
                    e.Components[ComponentRegistry<SendComponent>.Index] = null;
                }

                Entities.AddEntity(e);
            }
        }

        public void Handle(PlayerLeftMessage message)
        {
            PlayerLeft?.Invoke(message.PlayerId);
            foreach (var id in message.EntityIds)
                Simulation.Entities.RemoveEntity(id);
        }

        // Server Only
        public void Handle(ReadWorldsMessageRequest message) { throw new NotSupportedException(); }
        void IClientRpcMessageHandler.Handle(StartSimulationMessageRequest message) { throw new NotSupportedException(); }
        void IClientRpcMessageHandler.Handle(CreateWorldMessageRequest message) { throw new NotSupportedException(); }
        void IClientRpcMessageHandler.Handle(GetOtherPlayersMessageRequest message) { throw new NotSupportedException(); }
        void IClientRpcMessageHandler.Handle(ChangeMapMessageRequest message) { throw new NotSupportedException(); }
        void IClientRpcMessageHandler.Handle(JoinWorldMessageRequest message) { throw new NotSupportedException(); }
        void IClientRpcMessageHandler.Handle(LoginMessageRequest message) { throw new NotSupportedException(); }


    }
}