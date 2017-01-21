using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Maps;
using Misana.Core.Components;
using Misana.Core.Components.Events;
using Misana.Core.Components.StatusComponent;
using Misana.Core.Effects.BaseEffects;
using Misana.Core.Effects.Conditions;
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
using Misana.Core.Entities.Events;
using Misana.Core.Events.OnUse;
using Misana.Core.Events.Entities;
using Misana.Core.Systems;
using Misana.Core.Systems.StatusSystem;

namespace Misana.Core
{
    public class World : ISimulation
    {
        public ISimulation BaseSimulation { get; private set; }

        public Map CurrentMap => BaseSimulation.CurrentMap;

        public EntityManager Entities => BaseSimulation.Entities;

        public SimulationState State => BaseSimulation.State;

        public World(List<BaseSystem> beforSystems,List<BaseSystem> afterSystems)
        {
            BaseSimulation = new SimulationClient(beforSystems,afterSystems);
        }

        public void ChangeMap(Map map)
        {
            BaseSimulation.ChangeMap(map);
        }

        public void CreateEntity(string definitionName)
        {
            BaseSimulation.CreateEntity(definitionName);
        }

        public void CreateEntity(EntityDefinition defintion)
        {
            BaseSimulation.CreateEntity(defintion);
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
