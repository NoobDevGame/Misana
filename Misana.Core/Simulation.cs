using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class Simulation : ISimulation
    {

        private EntityCollidingMoverSystem _collidingMoverSystem;
        private EntityInteractionSystem _interactionSystem;
        private WieldedWieldableSystem _wieldedWieldableSystem;

        public EntityManager Entities { get; private set; }

        public Map CurrentMap { get; private set; }

        public SimulationState State { get; private set; }

        public Simulation(List<BaseSystem> beforSystems,List<BaseSystem> afterSystems)
        {
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

        public void ChangeMap(Map map)
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
                    EntityCreator.CreateEntity(Entities, CurrentMap,entity.Definition).Commit(Entities);
                }
            }

            EntityDefinition testDefinition = new EntityDefinition("DamageDealer");
            testDefinition.Definitions.Add(new TransformDefinition(new Vector2(2.5f,2.5f),CurrentMap.StartArea, 0.5f));

            testDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(0,0)));

            var colliderDef = new EntityColliderDefinition();
            colliderDef.OnCollisionEvents.Add(new MultiEvent(new Events.Conditions.FlagCondition("DamageDealer_Flag",true),
                    new ApplyEffectEvent(new DamageEffect(20f)) { ApplyTo = ApplicableTo.Other},
                    new ApplyEffectEvent(new TeleportEffect(5, 5, CurrentMap.StartArea.Id)) { ApplyTo = ApplicableTo.Other },
                    new ApplyEffectEvent(new SetEntityFlagEffect("DamageDealer_Flag")) { ApplyTo = ApplicableTo.Other }) {ApplyTo = ApplicableTo.Both,Debounce = TimeSpan.FromMilliseconds(250)}
            );
            testDefinition.Definitions.Add(colliderDef);

            var interactDef = new EntityInteractableDefinition();
            interactDef.OnInteractEvents.Add(new ApplyEffectEvent(new DamageEffect(20f)) { ApplyTo = ApplicableTo.Other,Debounce = TimeSpan.FromSeconds(1),CoolDown = TimeSpan.FromMilliseconds(250)});
            testDefinition.Definitions.Add(interactDef);



            EntityCreator.CreateEntity(Entities, CurrentMap, testDefinition)
                .Commit(Entities);
        }

        public void CreateEntity(string definitionName)
        {
            var definition = CurrentMap.GlobalEntityDefinitions["Player"];
            CreateEntity(definition);
        }

        public void CreateEntity(EntityDefinition defintion)
        {
            var entityBuilder = EntityCreator.CreateEntity(defintion, CurrentMap, new EntityBuilder());
            entityBuilder.Commit(Entities);
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            var playerDefinition = CurrentMap.GlobalEntityDefinitions["Player"];

            transform.CurrentArea = CurrentMap.StartArea;
            transform.Position = new Vector2(5, 3);

            var playerBuilder = EntityCreator.CreateEntity(playerDefinition, CurrentMap, new EntityBuilder())
                .Add<FacingComponent>()
                .Add(transform)
                .Add(input)
                ;

            /*
            var bow = new EntityBuilder()
                .Add<WieldableComponent>(wieldable => {
                    wieldable.OnUseEvents.Add(new ApplyEffectOnUseEvent(new SpawnProjectileEffect {
                        Builder = new EntityBuilder()
                            .Add<EntityColliderComponent>(pcoll => {
                                pcoll.OnCollisionEvents.Add(new ApplyEffectEvent(new DamageEffect(10)) { ApplyTo = ApplicableTo.Other });
                                //pcoll.OnCollisionEvents.Add(new ApplyEffectOnCollisionEvent(new RemoveEntityEffect()) {ApplyTo = ApplicableTo.Self});
                            })
                            .Add<SpriteInfoComponent>(),
                        Radius = 0.3f,
                        Expiration = 1500,
                        Speed = 0.25f

                    }) { CoolDown = TimeSpan.FromMilliseconds(250) });
                })
                .Add<SpriteInfoComponent>()
                .Add<WieldedComponent>(x => x.Offset = new Vector2(0.5f,0.5f))
                .Add<FacingComponent>()
                .Add<TransformComponent>(
                    x => {
                        x.ParentEntityId = playerId;
                        x.Position = new Vector2(0.3f,0.3f);
                    })
                .Commit(Entities);
            */

            var playerId = playerBuilder.Commit(Entities).Id;

            return playerId;
        }

        public void Update(GameTime gameTime)
        {
            Entities.Update(gameTime);
        }
    }
}
