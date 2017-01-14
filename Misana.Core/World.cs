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

namespace Misana.Core
{
    public class World
    {
        public int Simulation { get; private set; }

        public Map CurrentMap { get; private set; }

        public EntityManager Entities { get; }

        public World(EntityManager manager)
        {
            Entities = manager;
        }

        public void ChangeMap(Map map)
        {
            CurrentMap = map;
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


            EntityCreator.CreateEntity(playerDefinition, playerEntity);

            return playerEntity.Id;
        }

        public void Update(GameTime gameTime)
        {
            Entities.Update(gameTime);
        }
    }
}
