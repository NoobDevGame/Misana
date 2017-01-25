using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Communication.Components;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.BaseEffects;
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
using Misana.Core.Entities.Events;
using Misana.Core.Events.Entities;
using Misana.Core.Events.OnUse;
using Misana.Core.Maps;
using Misana.Core.Systems;
using Misana.Core.Systems.StatusSystem;

namespace Misana.Core
{
    public class Simulation : ISimulation
    {

        private EntityCollidingMoverSystem _collidingMoverSystem;
        private EntityInteractionSystem _interactionSystem;
        private WieldedWieldableSystem _wieldedWieldableSystem;

        public EntityManager Entities { get; private set; }

        public Map CurrentMap { get; private set; }

        public SimulationState State { get; private set; }

        public SimulationMode Mode { get; private set; }

        public Simulation(SimulationMode mode,List<BaseSystem> beforSystems,List<BaseSystem> afterSystems)
        {
            Mode = mode;
            State = SimulationState.Unloaded;

            _collidingMoverSystem = new EntityCollidingMoverSystem();
            _interactionSystem = new EntityInteractionSystem();
            _wieldedWieldableSystem = new WieldedWieldableSystem();

            List<BaseSystem> systems = new List<BaseSystem>();
            if (beforSystems != null)
                systems.AddRange(beforSystems);
            systems.Add(new InputSystem());
            systems.Add(_collidingMoverSystem);
            systems.Add(_interactionSystem);
            systems.Add(new BlockCollidingMoverSystem());
            systems.Add(new WieldedSystem());
            systems.Add(_wieldedWieldableSystem);
            systems.Add(new ProjectileSystem());
            systems.Add(new MoverSystem());
            systems.Add(new TimeDamageSystem());
            systems.Add(new ExpirationSystem());
            if (afterSystems != null)
                systems.AddRange(afterSystems);


            Entities = EntityManager.Create("LocalWorld",systems);
        }

        public async Task ChangeMap(Map map)
        {
            CurrentMap = map;

            Entities.Clear();
            _collidingMoverSystem.ChangeSimulation(this);
            _interactionSystem.ChangeSimulation(this);
            _wieldedWieldableSystem.ChangeSimulation(this);

            foreach (var area in CurrentMap.Areas)
            {
                foreach (var entity in area.Entities)
                {

                    var entityBuilder = EntityCreator.CreateEntity(Entities, CurrentMap,entity.Definition);

                    if (Mode == SimulationMode.Server)
                        entityBuilder.Add<SendComponent>();

                    entityBuilder.Commit(Entities);
                }
            }
        }

        public void CreateEntity(string definitionName)
        {
            var definition = CurrentMap.GlobalEntityDefinitions["Player"];
            CreateEntity(definition);
        }

        public void CreateEntity(EntityDefinition defintion)
        {
            var entityBuilder = EntityCreator.CreateEntity(defintion, CurrentMap, new EntityBuilder());
            if (Mode == SimulationMode.Local || Mode == SimulationMode.Server)
                entityBuilder.Add<SendComponent>();

            entityBuilder.Commit(Entities);
        }

        public void CreateEntity(EntityDefinition defintion,int entityId)
        {
            var entityBuilder = EntityCreator.CreateEntity(defintion, CurrentMap, new EntityBuilder());
            entityBuilder.Commit(Entities,entityId);
        }

        public void CreateEntity(int defintionId, int entityId)
        {
            var definition = CurrentMap.GlobalEntityDefinitions.First(i => i.Value.Id == defintionId).Value;
            CreateEntity(definition,entityId);
        }

        public async Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            if (Mode == SimulationMode.Server || Mode == SimulationMode.Local)
                throw new NotSupportedException();

            var playerDefinition = CurrentMap.GlobalEntityDefinitions["Player"];

            transform.CurrentArea = CurrentMap.StartArea;
            transform.Position = new Vector2(5, 3);

            var playerBuilder = EntityCreator.CreateEntity(playerDefinition, CurrentMap, new EntityBuilder())
                .Add<FacingComponent>()
                .Add(transform)
                .Add(input)
                ;

            if (Mode == SimulationMode.Local)
                playerBuilder.Add<SendComponent>();

            var playerId = playerBuilder.Commit(Entities).Id;

            return playerId;
        }

        public async Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform, int playerId)
        {
            if (Mode == SimulationMode.Server || Mode == SimulationMode.SinglePlayer)
                throw new NotSupportedException();

            var playerDefinition = CurrentMap.GlobalEntityDefinitions["Player"];

            transform.CurrentArea = CurrentMap.StartArea;
            transform.Position = new Vector2(5, 3);

            var playerBuilder = EntityCreator.CreateEntity(playerDefinition, CurrentMap, new EntityBuilder())
                    .Add<FacingComponent>()
                    .Add(transform)
                    .Add(input)
                ;

            if (Mode == SimulationMode.Local)
                playerBuilder.Add<SendComponent>();

            playerBuilder.Commit(Entities,playerId);

            return playerId;
        }

        public async Task Start()
        {
            State = SimulationState.Running;
        }

        public void Update(GameTime gameTime)
        {
            if (State == SimulationState.Running)
                Entities.Update(gameTime);
        }


    }
}
