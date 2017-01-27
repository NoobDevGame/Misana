using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Communication;
using Misana.Core.Communication.Components;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.BaseEffects;
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
using Misana.Core.Entities.Events;
using Misana.Core.Events;
using Misana.Core.Events.Entities;
using Misana.Core.Events.OnUse;
using Misana.Core.Maps;
using Misana.Core.Systems;
using Misana.Core.Systems.StatusSystem;
using Misana.Network;

namespace Misana.Core
{
    public class Simulation : ISimulation
    {
        public NetworkEffectMessenger EffectMessenger { get; }

        private EntityCollidingMoverSystem _collidingMoverSystem;
        private EntityInteractionSystem _interactionSystem;
        private WieldedWieldableSystem _wieldedWieldableSystem;
        private PositionTrackingSystem _positionTrackingSystem;

        public EntityManager Entities { get; private set; }

        public Map CurrentMap { get; private set; }

        public SimulationState State { get; private set; }

        public SimulationMode Mode { get; private set; }



        public Simulation(SimulationMode mode,List<BaseSystem> beforSystems,List<BaseSystem> afterSystems
            , INetworkSender sender,INetworkReceiver receiver)
        {
            EffectMessenger = new NetworkEffectMessenger(this,sender,receiver);
            _positionTrackingSystem = new PositionTrackingSystem();
            _collidingMoverSystem = new EntityCollidingMoverSystem(_positionTrackingSystem);
            Mode = mode;
            State = SimulationState.Unloaded;
            
            _interactionSystem = new EntityInteractionSystem();
            _wieldedWieldableSystem = new WieldedWieldableSystem();

            List<BaseSystem> systems = new List<BaseSystem>();
            if (beforSystems != null)
                systems.AddRange(beforSystems);

            systems.Add(_positionTrackingSystem);
            systems.Add(new InputSystem(_positionTrackingSystem,this));
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
            _positionTrackingSystem.ChangeMap(map);


            foreach (var area in CurrentMap.Areas)
            {
                foreach (var entity in area.Entities)
                {

                    await CreateEntity(entity.Definition, b =>
                    {
                        if (Mode == SimulationMode.Server)
                            b.Add<SendComponent>();
                    }, null);
                }
            }
        }

        public Task<int> CreateEntity(string definitionName, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var definition = CurrentMap.GlobalEntityDefinitions[definitionName];
            return CreateEntity(definition,createCallback, createdCallback);
        }

        public Task<int> CreateEntity(string definitionName, int entityId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var definition = CurrentMap.GlobalEntityDefinitions[definitionName];
            return CreateEntity(definition,entityId,createCallback, createdCallback);
        }

        public async Task<int> CreateEntity(EntityDefinition definition, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var entityBuilder = EntityCreator.CreateEntity(definition, CurrentMap, new EntityBuilder());

            createCallback?.Invoke(entityBuilder);

            var entity = entityBuilder.Commit(Entities);

            createdCallback?.Invoke(entity);

            StartCreateEvent(entity);

            return await Task.FromResult( entity.Id);
        }

        private void StartCreateEvent(Entity entity)
        {
            var createComponent = entity.Get<CreateComponent>();

            if (createComponent != null)
            {
                foreach (var createEvent in createComponent.OnCreateEvent)
                {
                    createEvent.Apply(Entities,entity,null,this);
                }
            }
        }

        public async Task<int> CreateEntity(EntityDefinition definition,int entityId,Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var entityBuilder = EntityCreator.CreateEntity(definition, CurrentMap, new EntityBuilder());

            createCallback?.Invoke(entityBuilder);

            var entity = entityBuilder.Commit(Entities,entityId);
            createdCallback?.Invoke(entity);

            StartCreateEvent(entity);

            return await Task.FromResult( entity.Id) ;
        }

        public Task<int> CreateEntity(int defintionId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var definition = CurrentMap.GlobalEntityDefinitions.First(i => i.Value.Id == defintionId).Value;
            return CreateEntity(definition,createCallback,createdCallback);
        }

        public Task<int> CreateEntity(int defintionId, int entityId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
        {
            var definition = CurrentMap.GlobalEntityDefinitions.First(i => i.Value.Id == defintionId).Value;
            return CreateEntity(definition,entityId,createCallback,createdCallback);
        }


        /*
        public async Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform,Action<EntityBuilder> createCallback,Action<Entity> createdCallback)
        {
            if (Mode == SimulationMode.Server || Mode == SimulationMode.Local)
                throw new NotSupportedException();

            var playerDefinition = CurrentMap.GlobalEntityDefinitions["Player"];

            transform.CurrentArea = CurrentMap.StartArea;
            transform.Position = new Vector2(5, 3);

            if (Mode == SimulationMode.Local)
                playerBuilder.Add<SendComponent>();

            createCallback?.Invoke(playerBuilder);

            var player = playerBuilder.Commit(Entities);

            createdCallback?.Invoke(player);

            var createComponent = player.Get<CreateComponent>();

            if (createComponent != null)
            {
                foreach (var createEvent in createComponent.OnCreateEvent)
                {
                    createEvent.Apply(Entities,player,null,this);
                }
            }

            return player.Id;
        }

        public async Task<int> CreatePlayer(PlayerInputComponent input, TransformComponent transform, int playerId, Action<EntityBuilder> createCallback, Action<Entity> createdCallback)
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

            createCallback?.Invoke(playerBuilder);

            var entity =  playerBuilder.Commit(Entities,playerId);

            createdCallback?.Invoke(entity);

            return playerId;
        }
        */

        public async Task Start()
        {
            State = SimulationState.Running;
        }

        public void Update(GameTime gameTime)
        {
            if (State == SimulationState.Running)
            {
                Entities.Update(gameTime);
            }


        }


    }
}
