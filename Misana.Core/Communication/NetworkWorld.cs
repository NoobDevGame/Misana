using System.Collections.Generic;
using System.Linq;
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
            var definition = CurrentMap.GlobalEntityDefinitions.First(i => i.Value.Id == message.DefinitionId).Value;
            BaseSimulation.CreateEntity(definition);

        }

        public void ChangeMap(Map map)
        {
            BaseSimulation.ChangeMap(map);
        }

        public void CreateEntity(string definitionName)
        {
            var definition = CurrentMap.GlobalEntityDefinitions.First(i => i.Key == definitionName).Value;
            CreateEntity(definition);
        }

        public void CreateEntity(EntityDefinition defintion)
        {
            CreateEntityMessage message = new CreateEntityMessage();
            message.DefinitionId = defintion.Id;
            SendMessage(ref message);
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