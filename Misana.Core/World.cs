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

        public void ChangeMap(Map map)
        {
            CurrentMap = map;

            Entities.Clear();

            foreach (var area in CurrentMap.Areas)
            {
                foreach (var entity in area.Entities)
                {
                    EntityCreator.CreateEntity(Entities, CurrentMap,entity.Definition);
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

            
            Entities.NewEntity()
               .Add<PositionComponent>(p =>
               {
                   p.CurrentArea = CurrentMap.StartArea;
                   p.Position = new Vector2(4, 3);
               })
               .Add<DimensionComponent>(p =>
               {
                   p.Radius = 0.5f;
               })
               .Add<MotionComponent>()
               .Add<BlockColliderComponent>()
               .Add<HealthComponent>(h => {
                   h.Max = 500;
                   h.Current = 500;
               })
               .Add<EntityCollider>(e => { e.Blocked = true; })
               .Add<CharacterComponent>(p =>
               {
                   p.Name = "Heidi";
               })
               .Add<CharacterRenderComponent>(p =>
               {
                   p.TilePosition = new Index2(0, 9);
               })
               .Commit();
        }

        public int CreatePlayer(PlayerInputComponent input, TransformComponent transform)
        {
            EntityDefinition playerDefinition = new EntityDefinition();
            playerDefinition.Definitions.Add(new DimensionDefinition());
            playerDefinition.Definitions.Add(new HealtDefinition());
            playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1,9)));
            playerDefinition.Definitions.Add(new MotionComponentDefinition());
            playerDefinition.Definitions.Add(new EntityColliderDefinition());
            playerDefinition.Definitions.Add(new BlockColliderDefinition());
            playerDefinition.Definitions.Add(new EntityFlagDefintion());
            position.CurrentArea = CurrentMap.StartArea;
            position.Position = position.CurrentArea.SpawnPoint;

            var playerEntity = Entities.NewEntity()
                .Add(position)
                .Add(input);


            EntityCreator.CreateEntity(playerDefinition, CurrentMap, playerEntity);

            return playerEntity.Id;
        }

        public void Update(GameTime gameTime)
        {
            Entities.Update(gameTime);
        }
    }
}
