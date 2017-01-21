﻿using System.Collections.Generic;
using Misana.Core.Communication.Messages;
using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Maps;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkWorld : InternNetworkClient, ISimulation
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        public SimulationState State => BaseSimulation.State;

        public NetworkWorld()
        {
            List<BaseSystem> beforSystems = new List<BaseSystem>();
            beforSystems.Add(new ReceiveEntityPositionSystem(OuterClient));

            List<BaseSystem> afterSystems = new List<BaseSystem>();
            afterSystems.Add(new SendEntityPositionSystem(OuterClient));

            BaseSimulation = new Simulation(beforSystems,afterSystems);

            OuterClient.RegisterOnMessageCallback<CreateEntityMessage>(OnCreateEntity);
        }

        private void OnCreateEntity(CreateEntityMessage message)
        {
        }

        public void ChangeMap(Map map)
        {
            BaseSimulation.ChangeMap(map);
        }

        public void CreateEntity(string definitionName)
        {
            CreateEntityMessage message = new CreateEntityMessage();

            SendMessage(ref message);
        }

        public void CreateEntity(EntityDefinition defintion)
        {
            CreateEntity(defintion.Name);
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            return BaseSimulation.CreatePlayer(input, transform);
        }

        public void Update(GameTime gameTime)
        {
            BaseSimulation.Update(gameTime);
        }
    }
}