﻿using Misana.Core.Ecs;
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
        private EntityInteractionSystem _interactionSystem;

        public World(List<BaseSystem> afterSystems)
        {
            _collidingMoverSystem = new EntityCollidingMoverSystem();
            _interactionSystem = new EntityInteractionSystem();

            List<BaseSystem> systems = new List<BaseSystem>();
            systems.Add(new InputSystem());
            systems.Add(_collidingMoverSystem);
            systems.Add(_interactionSystem);
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
            _interactionSystem.ChangeWorld(this);
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
            colliderDef.OnCollisionEvents.Add(new MultiOnCollisionEvent(new Events.Conditions.FlagCondition("DamageDealer_Flag",true),
                new ApplyEffectOnCollisionEvent(new DamageEffect(20f)) { ApplyTo = ApplicableTo.Other},
                new ApplyEffectOnCollisionEvent(new TeleportEffect(5, 5, CurrentMap.StartArea.Id)) { ApplyTo = ApplicableTo.Other },
                new ApplyEffectOnCollisionEvent(new SetEntityFlagEffect("DamageDealer_Flag")) { ApplyTo = ApplicableTo.Other }) {ApplyTo = ApplicableTo.Both,Debounce = TimeSpan.FromMilliseconds(250)}
            );
            testDefinition.Definitions.Add(colliderDef);




            EntityCreator.CreateEntity(Entities, CurrentMap, testDefinition)
                .Add<EntityInteractableComponent>(p =>
                {
                    p.OnInteractionEvents.Add(new ApplyEffectOnCollisionEvent(new DamageEffect(20f)) { ApplyTo = ApplicableTo.Other,Debounce = TimeSpan.FromSeconds(1),CoolDown = TimeSpan.FromMilliseconds(250)});
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
            playerDefinition.Definitions.Add(new EntityFlagDefintion());

            transform.CurrentArea = CurrentMap.StartArea;
            transform.Position = new Vector2(5, 3);

            var playerBuilder = EntityCreator.CreateEntity(playerDefinition, CurrentMap, new EntityBuilder()
                .Add(transform)
                .Add(input))
                .Add<EntityInteractableComponent>();

            return playerBuilder.Commit(Entities).Id;
        }

        public void Update(GameTime gameTime)
        {
            Entities.Update(gameTime);
        }
    }
}
