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
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
using Misana.Core.Entities.Events;
using Misana.Core.Events.BaseEvents;
using Misana.Core.Events.Conditions;
using Misana.Core.Systems;
using Misana.Core.Systems.StatusSystem;

namespace Misana.Core
{
    public class World
    {
        public int Simulation { get; private set; }

        public Map CurrentMap { get; private set; }

        public EntityManager Entities { get; }

        public World(List<BaseSystem> afterSystems)
        {
            List<BaseSystem> systems = new List<BaseSystem>();
            systems.Add(new InputSystem());
            systems.Add(new EntityCollidingMoverSystem());
            systems.Add(new BlockCollidingMoverSystem());
            systems.Add(new MoverSystem());
            systems.Add(new CollisionApplicatorSystem(this));
            systems.Add(new EntityCollisionRemoverSystem());
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

            foreach (var area in CurrentMap.Areas)
            {
                foreach (var entity in area.Entities)
                {
                    EntityCreator.CreateEntity(Entities, CurrentMap,entity.Definition).Commit(Entities);
                }
            }

            EntityDefinition testDefinition = new EntityDefinition("DamageDealer");
            testDefinition.Definitions.Add(new DimensionDefinition(0.5f));
            testDefinition.Definitions.Add(new PositionDefinition(new Vector2(2.5f,2.5f),CurrentMap.StartArea));
            testDefinition.Definitions.Add(new EntityColliderDefinition());
            testDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(0,0)));

            CollisionDefinition collision = new CollisionDefinition(new FlagCondition("DamageDealer_Flag",true));
            collision.EventsActions.Add(new DamageEvent(20f));
            collision.EventsActions.Add(new TeleportEvent(5,5,CurrentMap.StartArea.Id));
            collision.EventsActions.Add(new SetEntityFlagEvent("DamageDealer_Flag"));
            testDefinition.Definitions.Add(collision);

            EntityCreator.CreateEntity(Entities, CurrentMap, testDefinition);

            
                new EntityBuilder()
                    .Add<TransformComponent>(
                        p => {
                            p.CurrentArea = CurrentMap.StartArea;
                            p.Position = new Vector2(4, 3);
                            p.Radius = 0.5f;
                        })
                    .Add<MotionComponent>()
                    .Add<BlockColliderComponent>()
                    .Add<HealthComponent>(
                        h => {
                            h.Max = 500;
                            h.Current = 500;
                        })
                    .Add<EntityColliderComponent>(e => { e.Blocked = true; })
                    .Add<CharacterComponent>(
                        p => {
                            p.Name = "Heidi";
                        })
                    .Add<CharacterRenderComponent>(
                        p => {
                            p.TilePosition = new Index2(0, 9);
                        })
                    .CommitAndReturnCopy(Entities)
                    .Add<TransformComponent>(p => {
                        p.CurrentArea = CurrentMap.StartArea;
                        p.Position = new Vector2(2, 3);
                        p.Radius = 0.5f;
                    })
                    .Add<CharacterComponent>(p => {
                        p.Name = "Heidi 2";
                    })
                    .Add<EntityColliderComponent>(
                        e => {
                            e.Blocked = true;
                            e.CollisionEffects.Add(new SimpleAddComponentCollisionEffect<TimeDamageComponent>(
                                new TimeDamageComponent {
                                    DamagePerSeconds = 5,
                                    EffectTime = TimeSpan.FromSeconds(10)
                                }) {
                                ApplyTo = CollisionEffect.ApplicableTo.Both,
                                Debounce = TimeSpan.FromMilliseconds(100)
                            });

                            e.CollisionEffects.Add(new CustomCodeCollisionEffect(
                                (em, self, other) =>
                                {
                                    new EntityBuilder()
                                        .Add<TransformComponent>(
                                            tf =>
                                            {
                                                var otf = self.Get<TransformComponent>();
                                                tf.CurrentArea = otf.CurrentArea;
                                                tf.Position = new Vector2(Random.Next(0, 101), Random.Next(0, 101));
                                            })
                                        .Add<MotionComponent>()
                                        .Add<EntityColliderComponent>(
                                            ec =>
                                            {
                                                ec.Blocked = false;
                                                ec.CollisionEffects.Add(
                                                    new SimpleAddComponentCollisionEffect<TimeDamageComponent>(
                                                        new TimeDamageComponent
                                                        {
                                                            DamagePerSeconds = Random.Next(-11, 15),
                                                            EffectTime = TimeSpan.FromSeconds(1)
                                                        })
                                                    {
                                                        ApplyTo = CollisionEffect.ApplicableTo.Other,
                                                        Debounce = TimeSpan.FromMilliseconds(100)
                                                    });
                                            })
                                            .Commit(em);
                                })
                            {
                                Debounce = TimeSpan.FromMilliseconds(5)
                            });
                        })
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
