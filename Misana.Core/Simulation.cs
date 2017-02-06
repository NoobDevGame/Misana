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
using Misana.Core.Network;
using Misana.Core.Systems;
using Misana.Core.Systems.StatusSystem;

namespace Misana.Core
{
    public class Simulation : ISimulation
    {
        private readonly IOutgoingMessageQueue _queue;

        private EntityCollidingMoverSystem _collidingMoverSystem;
        private EntityInteractionSystem _interactionSystem;
        private WieldedWieldableSystem _wieldedWieldableSystem;
        private PositionTrackingSystem _positionTrackingSystem;
        private BlockCollidingMoverSystem _blockCollidingMoverSystem;
        public SpawnerSystem SpawnerSystem { get; private set; }

        public EntityManager Entities { get; private set; }

        public Map CurrentMap { get; private set; }

        public SimulationState State { get; private set; }

        public SimulationMode Mode { get; private set; }

        public Simulation(SimulationMode mode,List<BaseSystem> beforSystems,List<BaseSystem> afterSystems
            ,IOutgoingMessageQueue queue, int start)
        {
            _queue = queue;
            //_sender = sender;
           // EffectMessenger = new EffectApplicator(this,sender,receiver);
            _positionTrackingSystem = new PositionTrackingSystem();
            _collidingMoverSystem = new EntityCollidingMoverSystem(_positionTrackingSystem);
            Mode = mode;
            State = SimulationState.Unloaded;
            
            _interactionSystem = new EntityInteractionSystem();
            _wieldedWieldableSystem = new WieldedWieldableSystem();
            _blockCollidingMoverSystem = new BlockCollidingMoverSystem();
            SpawnerSystem = new SpawnerSystem();

            List<BaseSystem> systems = new List<BaseSystem>();
            if (beforSystems != null)
                systems.AddRange(beforSystems);

            systems.Add(_positionTrackingSystem);
            systems.Add(new InputSystem(_positionTrackingSystem,this));
            systems.Add(_collidingMoverSystem);
            systems.Add(_interactionSystem);
            systems.Add(new WieldedSystem());
            systems.Add(_wieldedWieldableSystem);
            systems.Add(new ProjectileSystem());
            systems.Add(_blockCollidingMoverSystem);
            systems.Add(new MoverSystem());
            systems.Add(new TimeDamageSystem());
            systems.Add(new ExpirationSystem());
            systems.Add(SpawnerSystem);
            if (afterSystems != null)
                systems.AddRange(afterSystems);


            Entities = EntityManager.Create("LocalWorld",systems, mode, _queue);

            for (int i = start; i < start + 50000; i++)
            {
                Entities.AvailableEntityIds.Enqueue(i);
            }
        }

        public async Task ChangeMap(Map map)
        {
            CurrentMap = map;

            Entities.Clear();
            _collidingMoverSystem.ChangeSimulation(this);
            _interactionSystem.ChangeSimulation(this);
            _wieldedWieldableSystem.ChangeSimulation(this);
            _positionTrackingSystem.ChangeMap(map);
            _blockCollidingMoverSystem.ChangeMap(map);

            int nextId = 1;

            if (Mode == SimulationMode.Server)
            {
                foreach (var area in CurrentMap.Areas)
                {
                    for (var i = 0; i < area.Entities.Count; i++)
                    {
                        var entity = area.Entities[i];
                        EntityCreator.CreateEntity(entity, map, new EntityBuilder(), this)
                            .Add<SendComponent>()
                            .Commit(Entities);
                    }
                }
            }


        }

        public Task Start()
        {
            State = SimulationState.Running;
            return Task.CompletedTask;
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
