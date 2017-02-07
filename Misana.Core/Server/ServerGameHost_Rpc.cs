using System;
using System.Collections.Generic;
using System.Threading;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Core.Entities;
using Misana.Core.Maps;
using Misana.Core.Network;

namespace Misana.Core.Server
{
    public partial class ServerGameHost : IServerRpcMessageHandler
    {
        void IServerRpcMessageHandler.Handle(StartSimulationMessageRequest message, byte messageId, IClientOnServer client)
        {
            var response = new StartSimulationMessageResponse(true);

            var simulation = players[client.NetworkId].Simulation;

            if (simulation.BaseSimulation.State == SimulationState.Running)
            {
                client.Respond(response, messageId);
                return;
            }

            if (simulation.Owner.NetworkId != client.NetworkId)
            {
                response.Result = false;
                client.Respond(response, messageId);
                return;
            }

            var entities = simulation.BaseSimulation.Entities.GetAllEntitiesSlowly();

            var playerDef = simulation.BaseSimulation.CurrentMap.GlobalEntityDefinitions["Player"];


            var playerEntities = new List<Entity>();
            var vPlayers = players.Values;
            foreach (var p in vPlayers)
            {
                playerEntities.Add(EntityCreator.CreateEntity(playerDef, simulation.BaseSimulation.CurrentMap, new EntityBuilder(), simulation.BaseSimulation)
                    .Commit(simulation.BaseSimulation.Entities));
            }

            entities.AddRange(simulation.BaseSimulation.Entities.PendingEntities);


            for (int i = 0; i < vPlayers.Count; i++)
            {
                vPlayers[i].Client.Send(new InitialGameState { Entities = entities, PlayerId = playerEntities[i].Id});
            }

            simulation.BaseSimulation.Start();

            client.Respond(response, messageId);
            Broadcast(new OnStartSimulationMessage(), client.NetworkId);
        }

        void IServerRpcMessageHandler.Handle(LoginMessageRequest message, byte messageId, IClientOnServer client)
        {

            var responseMessage = new LoginMessageResponse(client.NetworkId);

            players.Add(client.NetworkId,new NetworkPlayer(message.Name,client));



            client.Respond(responseMessage, messageId);

            ((IServerRpcMessageHandler)this).Handle((ReadWorldsMessageRequest)null,0, client);
        }

        void IServerRpcMessageHandler.Handle(JoinWorldMessageRequest message, byte messageId, IClientOnServer client)
        {
            JoinWorldMessageResponse response = new JoinWorldMessageResponse(simulation != null
                ,simulation.BaseSimulation.CurrentMap != null,simulation.BaseSimulation.CurrentMap?.Name, 50001 + players.Count * 50000);

            if (simulation != null)
            {
                var player = players[client.NetworkId];
                simulation.Players.Add(player);
                player.SetSimulation(simulation);


                ((IServerRpcMessageHandler)this).Handle((GetOtherPlayersMessageRequest)null, 0, client);
                client.Respond(response,messageId);


                if (simulation.BaseSimulation.State == SimulationState.Running)
                {
                    Thread.Sleep(250);
                    var entities = simulation.BaseSimulation.Entities.GetAllEntitiesSlowly();

                    var playerDef = simulation.BaseSimulation.CurrentMap.GlobalEntityDefinitions["Player"];


                    var playerEntities = new List<Entity>();
                    playerEntities.Add(EntityCreator.CreateEntity(playerDef, simulation.BaseSimulation.CurrentMap, new EntityBuilder(), simulation.BaseSimulation)
                        .Commit(simulation.BaseSimulation.Entities));


                    entities.AddRange(simulation.BaseSimulation.Entities.PendingEntities);

                    var newEntities = simulation.BaseSimulation.Entities.PendingEntities;


                    client.Send(new InitialGameState { Entities = entities, PlayerId = playerEntities[0].Id});

                    //simulation.BaseSimulation.Start();

                   // client.Respond(response, messageId);

                    Broadcast(new HotJoinedMessage {
                        Entities = newEntities,
                        Name = player.Name,
                        PlayerId = player.NetworkId
                    }, client.NetworkId);
                }
                else
                {
                    OnJoinWorldMessage joinMessage = new OnJoinWorldMessage(player.NetworkId,player.Name);
                    Broadcast(joinMessage,client.NetworkId);
                }
            }
            else
            {
                client.Respond(response,messageId);
            }


        }

        void IServerRpcMessageHandler.Handle(GetOtherPlayersMessageRequest message, byte messageId, IClientOnServer client)
        {
            foreach (var player in simulation.Players)
            {
                if (player.NetworkId == client.NetworkId)
                    continue;
                var response = new PlayerInfoMessage(player.NetworkId, player.Name);
                client.Respond(response, messageId);
            }
        }

        void IServerRpcMessageHandler.Handle(CreateWorldMessageRequest message, byte messageId, IClientOnServer client)
        {
            var networkPlayer = players[client.NetworkId];

            simulation = new NetworkSimulation(networkPlayer,null,null, this);
            simulation.Name = message.Name;
            networkPlayer.SetSimulation(simulation);

            CreateWorldMessageResponse messageResponse = new CreateWorldMessageResponse(true,simulation.Id, 50001);
            client.Respond(messageResponse, messageId);
        }


        void IServerRpcMessageHandler.Handle(ChangeMapMessageRequest message, byte messageId, IClientOnServer client)
        {
            ChangeMapMessageResponse response = new ChangeMapMessageResponse(true);

            var simulation = players[client.NetworkId].Simulation;

            if (simulation.Owner.NetworkId != client.NetworkId)
            {
                response.Result = false;
                client.Respond(response, messageId);
                return;
            }

            var map = MapLoader.Load(message.Name);

            simulation.BaseSimulation.ChangeMap(map);


            client.Respond(response, messageId);
        }

        void IServerRpcMessageHandler.Handle(ReadWorldsMessageRequest message, byte messageId, IClientOnServer client)
        {
            if(simulation == null)
                return;

            WorldInformationMessage response = new WorldInformationMessage(simulation.Id,simulation.Name);
            client.Respond(response, messageId);
        }


        // Client Only
        void IServerRpcMessageHandler.Handle(LoginMessageResponse message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(JoinWorldMessageResponse message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(OnJoinWorldMessage message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(PlayerInfoMessage message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(CreateWorldMessageResponse message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(ChangeMapMessageResponse message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(WorldInformationMessage message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(InitialGameState message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(HotJoinedMessage message, byte messageId, IClientOnServer client)  { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(PlayerLeftMessage message, byte messageId, IClientOnServer client)  { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(StartSimulationMessageResponse message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
        void IServerRpcMessageHandler.Handle(OnStartSimulationMessage message, byte messageId, IClientOnServer client) { throw new NotSupportedException(); }
    }
}