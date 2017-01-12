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

        public EntityManager Entities { get; }

        public World(EntityManager manager)
        {
            Entities = manager;
        }

        public void ChangeMap(Map map)
        {
            CurrentMap = map;

            Entities.NewEntity()
                .Add<PositionComponent>(p =>
                {
                    p.CurrentArea = CurrentMap.StartArea;
                    p.Position = new Vector2(2, 3);  
                })
                .Add<DimensionComponent>(p =>
                {
                    p.Radius = 0.5f;
                })
                .Add<MotionComponent>()
                .Add<BlockColliderComponent>()
                .Add<HealthComponent>(h => { h.Max = 500;
                    h.Current = 250;
                })
                .Add<EntityCollider>(ec => { ec.AppliesSideEffect = true; })
                .Add<VelocityApplicator>(va => { va.Force = new Vector2(2.5f,0);})
                .Add<CharacterComponent>(p => {p.Name = "Heidi";})
                .Add<CharacterRenderComponent>(p =>
                {
                    p.TilePosition = new Index2(0, 9);
                })
                .Commit();

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
               .Add<EntityCollider>()
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

        public int CreatePlayer(PlayerInputComponent input,PositionComponent position)
        {
            position.CurrentArea = CurrentMap.StartArea;
            position.Position = new Vector2(5, 3);

            var entity =  Entities.NewEntity()
                .Add(position)
                .Add(input)
                .Add<DimensionComponent>(p => 
                {
                    p.Radius = 0.5f;
                })
                .Add<MotionComponent>()
                .Add<BlockColliderComponent>()
                .Add<EntityCollider>()
                .Add<CharacterRenderComponent>(p => 
                {
                    p.TilePosition = new Index2(1, 9);
                })
                .Commit();

            return entity.Id;
        }

        public void Update(GameTime gameTime)
        {
            Entities.Update(gameTime);
        }
    }
}
