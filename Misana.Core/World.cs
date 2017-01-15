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
using Misana.Core.Events.Collision;
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

        public World(List<BaseSystem> afterSystems)
        {
            _collidingMoverSystem = new EntityCollidingMoverSystem();

            List<BaseSystem> systems = new List<BaseSystem>();
            systems.Add(new InputSystem());
            systems.Add(_collidingMoverSystem);
            systems.Add(new BlockCollidingMoverSystem());
            systems.Add(new MoverSystem());
            systems.Add(new TimeDamageSystem());
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
            colliderDef.OnCollisionEvents.Add(new ApplyEffectOnCollisionEvent(new DamageEffect(20f), new FlagCondition("DamageDealer_Flag", true)) { ApplyTo = ApplicableTo.Other, Debounce = TimeSpan.FromMilliseconds(250)});
            colliderDef.OnCollisionEvents.Add(new ApplyEffectOnCollisionEvent(new TeleportEffect(5, 5, CurrentMap.StartArea.Id)) { ApplyTo = ApplicableTo.Other, Debounce = TimeSpan.FromMilliseconds(250) });
            colliderDef.OnCollisionEvents.Add(new ApplyEffectOnCollisionEvent(new SetEntityFlagEffect("DamageDealer_Flag")) { ApplyTo = ApplicableTo.Other, Debounce = TimeSpan.FromMilliseconds(250) });
            testDefinition.Definitions.Add(colliderDef);
            //CollisionDefinition collision = new CollisionDefinition(new FlagCondition("DamageDealer_Flag",true));
            //collision.EventsActions.Add(new DamageEffect(20f));
            //collision.EventsActions.Add(new TeleportEffect(5,5,CurrentMap.StartArea.Id));
            //collision.EventsActions.Add(new SetEntityFlagEffect("DamageDealer_Flag"));
            //testDefinition.Definitions.Add(collision);

            EntityCreator.CreateEntity(Entities, CurrentMap, testDefinition).Commit(Entities);

            
                //new EntityBuilder()
                //    .Add<TransformComponent>(
                //        p => {
                //            p.CurrentArea = CurrentMap.StartArea;
                //            p.Position = new Vector2(4, 3);
                //            p.Radius = 0.5f;
                //        })
                //    .Add<MotionComponent>()
                //    .Add<BlockColliderComponent>()
                //    .Add<HealthComponent>(
                //        h => {
                //            h.Max = 500;
                //            h.Current = 500;
                //        })
                //    .Add<EntityColliderComponent>(e => { e.Blocked = true; })
                //    .Add<CharacterComponent>(
                //        p => {
                //            p.Name = "Heidi";
                //        })
                //    .Add<CharacterRenderComponent>(
                //        p => {
                //            p.TilePosition = new Index2(0, 9);
                //        })
                //    .CommitAndReturnCopy(Entities)
                //    .Add<TransformComponent>(p => {
                //        p.CurrentArea = CurrentMap.StartArea;
                //        p.Position = new Vector2(2, 3);
                //        p.Radius = 0.5f;
                //    })
                //    .Add<CharacterComponent>(p => {
                //        p.Name = "Heidi 2";
                //    })
                //    .Add<EntityColliderComponent>(
                //        e => {
                //            e.Blocked = true;
                //            e.CollisionEffects.Add(new SimpleAddComponentCollisionEffect<TimeDamageComponent>(
                //                new TimeDamageComponent {
                //                    DamagePerSeconds = 5,
                //                    EffectTime = TimeSpan.FromSeconds(10)
                //                }) {
                //                ApplyTo = CollisionEffect.ApplicableTo.Both,
                //                Debounce = TimeSpan.FromMilliseconds(100)
                //            });

                //            e.CollisionEffects.Add(new CustomCodeCollisionEffect(
                //                (em, self, other) =>
                //                {
                //                    new EntityBuilder()
                //                        .Add<TransformComponent>(
                //                            tf =>
                //                            {
                //                                var otf = self.Get<TransformComponent>();
                //                                tf.CurrentArea = otf.CurrentArea;
                //                                tf.Position = new Vector2(Random.Next(0, 101), Random.Next(0, 101));
                //                            })
                //                        .Add<MotionComponent>()
                //                        .Add<EntityColliderComponent>(
                //                            ec =>
                //                            {
                //                                ec.Blocked = false;
                //                                ec.CollisionEffects.Add(
                //                                    new SimpleAddComponentCollisionEffect<TimeDamageComponent>(
                //                                        new TimeDamageComponent
                //                                        {
                //                                            DamagePerSeconds = Random.Next(-11, 15),
                //                                            EffectTime = TimeSpan.FromSeconds(1)
                //                                        })
                //                                    {
                //                                        ApplyTo = CollisionEffect.ApplicableTo.Other,
                //                                        Debounce = TimeSpan.FromMilliseconds(100)
                //                                    });
                //                            })
                //                            .Commit(em);
                //                })
                //            {
                //                Debounce = TimeSpan.FromMilliseconds(5)
                //            });
                //        })
                //    .Commit(Entities);
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            EntityDefinition playerDefinition = new EntityDefinition();
            playerDefinition.Definitions.Add(new HealthDefinition());
            playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1,9)));
            playerDefinition.Definitions.Add(new MotionComponentDefinition());
            playerDefinition.Definitions.Add(new EntityColliderDefinition());
            playerDefinition.Definitions.Add(new BlockColliderDefinition());

            transform.CurrentArea = CurrentMap.StartArea;
            transform.Position = new Vector2(5, 3);

            var playerBuilder = EntityCreator.CreateEntity(playerDefinition, CurrentMap, new EntityBuilder()
                .Add(transform)
                .Add(input));

            return playerBuilder.Commit(Entities).Id;
        }

        public void Update(GameTime gameTime)
        {
            Entities.Update(gameTime);
        }
    }
}
