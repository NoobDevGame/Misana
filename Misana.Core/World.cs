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
    public class World
    {
        public int Simulation { get; private set; }

        public Map CurrentMap { get; private set; }

        public EntityManager Entities { get; }

        private EntityCollidingMoverSystem _collidingMoverSystem;
        private EntityInteractionSystem _interactionSystem;
        private WieldedWieldableSystem _wieldedWieldableSystem;

        public World(List<BaseSystem> afterSystems)
        {
            _collidingMoverSystem = new EntityCollidingMoverSystem();
            _wieldedWieldableSystem = new WieldedWieldableSystem();
            _interactionSystem = new EntityInteractionSystem();

            List<BaseSystem> systems = new List<BaseSystem>();
            systems.Add(new InputSystem());
            systems.Add(_collidingMoverSystem);
            systems.Add(_interactionSystem);
            systems.Add(new BlockCollidingMoverSystem());
            systems.Add(_wieldedWieldableSystem);
            systems.Add(new WieldedSystem());
            systems.Add(new ProjectileSystem());
            systems.Add(new MoverSystem());
            systems.Add(new TimeDamageSystem());
            systems.Add(new ExpirationSystem());
            systems.AddRange(afterSystems);


            Entities = EntityManager.Create("LocalWorld",systems);
        }

        private Random Random = new Random();
        private static int foo;
        public void ChangeMap(Map map)
        {
            CurrentMap = map;

            Entities.Clear();
            _collidingMoverSystem.ChangeWorld(this);
            _interactionSystem.ChangeWorld(this);
            _wieldedWieldableSystem.ChangeWorld(this);

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

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            EntityDefinition playerDefinition = new EntityDefinition();
            playerDefinition.Definitions.Add(new HealthDefinition());
            playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1,9)));
            playerDefinition.Definitions.Add(new MotionComponentDefinition());
            playerDefinition.Definitions.Add(new EntityColliderDefinition());
            playerDefinition.Definitions.Add(new BlockColliderDefinition());
            playerDefinition.Definitions.Add(new EntityFlagDefintion());
            playerDefinition.Definitions.Add(new EntityInteractableDefinition());

            transform.CurrentArea = CurrentMap.StartArea;
            transform.Position = new Vector2(5, 3);


            var playerWielding = new WieldingComponent();

            var playerBuilder = EntityCreator.CreateEntity(playerDefinition, CurrentMap, new EntityBuilder()
                .Add<FacingComponent>()
                .Add(transform)
                .Add(input)
                .Add(playerWielding)
                );

            var playerId = playerBuilder.Commit(Entities).Id;

            var bow = new EntityBuilder()
                .Add<WieldableComponent>(wieldable => {
                    wieldable.OnUseEvents.Add(new ApplyEffectOnUseEvent(new SpawnProjectileEffect {
                      Builder = new EntityBuilder()
                        .Add<EntityColliderComponent>(pcoll => { 
                            pcoll.OnCollisionEvents.Add(new ApplyEffectEvent(new DamageEffect(10)) { ApplyTo = ApplicableTo.Other });
                            //pcoll.OnCollisionEvents.Add(new ApplyEffectOnCollisionEvent(new RemoveEntityEffect()) {ApplyTo = ApplicableTo.Self});
                        })
                        .Add<CharacterRenderComponent>(),
                      Radius = 0.3f,
                      Expiration = 2000,
                      Speed = 0.33f

                  }) { CoolDown = TimeSpan.FromMilliseconds(1) });      
                })
                .Add<CharacterRenderComponent>()
                .Add<EntityColliderComponent>()
                .Add<FacingComponent>()
                .Add<WieldingComponent>();



            var bowBow = bow.Copy();
            var bowEntity = bow.Add<WieldedComponent>(x => x.Offset = new Vector2(0.3f, 0.3f)).Add<TransformComponent>(
                x => {
                    x.ParentEntityId = playerId;
                    x.Position = new Vector2(-0.3f, 0.3f);
                })
            .Commit(Entities);

            var wielder = bowEntity;

            for (int i = 0; i < 5; i++)
            {
                var i1 = i;
                var bowBowEntity = bowBow.Copy()
                    .Add<WieldedComponent>(x => x.Offset = new Vector2(0.3f * i1, 0.3f))
                    .Add<TransformComponent>(x => {
                        x.ParentEntityId = wielder.Id;
                        x.Position = new Vector2(i1, 0.1f);
                    }).Commit(Entities);

                var bew = wielder.Get<WieldingComponent>();
                bew.Use = true;
                bew.RightHandEntityId = bowBowEntity.Id;
                wielder = bowBowEntity;
            }

            //var bowBowEntity = bowBow.Add<TransformComponent>(
            //    x => {
            //        x.ParentEntityId = bowEntity.Id;
            //        x.Position = new Vector2(0.3f, 0.3f);
            //    }).Commit(Entities);

            //var bew = bowEntity.Get<WieldingComponent>();
            //bew.Use = true;
            //bew.RightHandEntityId = bowBowEntity.Id;



            playerWielding.RightHandEntityId = bowEntity.Id;
            playerWielding.TwoHanded = true;

            return playerId;
        }

        public void Update(GameTime gameTime)
        {
            Entities.Update(gameTime);
        }
    }
}
