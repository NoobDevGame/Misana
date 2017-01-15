using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Maps;
using Misana.Core.Components;
using Misana.Core.Components.StatusComponent;
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
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
            systems.Add(new BlockCollidingMoverSystem());
            systems.Add(new MoverSystem());
            systems.Add(new CollisionApplicatorSystem());
            systems.Add(new EntityCollisionRemoverSystem());
            systems.Add(new TimeDamageSystem());
            systems.AddRange(afterSystems);


            Entities = EntityManager.Create("LocalWorld",systems);
        }

        public void ChangeMap(Map map)
        {
            CurrentMap = map;

            foreach (var area in CurrentMap.Areas)
            {
                foreach (var entity in area.Entities)
                {
                    EntityCreator.CreateEntity(Entities, CurrentMap,entity.Definition);
                }
            }

        }

        public int CreatePlayer(PlayerInputComponent input,PositionComponent position)
        {
            EntityDefinition playerDefinition = new EntityDefinition();
            playerDefinition.Definitions.Add(new DimensionDefinition());
            playerDefinition.Definitions.Add(new HealtDefinition());
            playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1,9)));
            playerDefinition.Definitions.Add(new MotionComponentDefinition());
            playerDefinition.Definitions.Add(new EntityColliderDefinition());
            playerDefinition.Definitions.Add(new BlockColliderDefinition());

            position.CurrentArea = CurrentMap.StartArea;
            position.Position = new Vector2(5, 3);

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
