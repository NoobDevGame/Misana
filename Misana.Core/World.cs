using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Maps;
using Misana.Core.Components;

namespace Misana.Core
{
    public class World
    {
        public int Simulation { get; private set; }

        public Map CurrentMap { get; private set; }

        public World()
        {

        }

        public void ChangeMap(Map map)
        {
            CurrentMap = map;

        }

        public Entity CreatePlayer()
        {
            var entity =  CurrentMap.Entities.NewEntity()
                .Add<PositionComponent>(p =>
                {
                    p.CurrentArea = CurrentMap.StartArea;
                    p.Position = new Vector2(5, 5);
                })
                .Add<PlayerInputComponent>()
                .Add<MotionComponent>().Commit();

            return entity;
        }

        public void Update(GameTime gameTime)
        {
            CurrentMap.Entities.Update(gameTime);
        }
    }
}
