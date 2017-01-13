using engenious;
using Misana.Controls;
using Misana.Core;
using Misana.Core.Ecs;
using Misana.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Systems.StatusSystem;
using Misana.Core.Maps;

namespace Misana.Components
{
    internal class SimulationComponent : GameComponent
    {
        public World World { get; private set; }

        public new MisanaGame Game;

        public EntityManager Entities { get; private set; }

        public CharacterRenderSystem CharacterRender { get; private set; }

        public SimulationComponent(MisanaGame game) : base(game)
        {
            Game = game;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            var foo = EntityManager.ComponentCount;
            CharacterRender = new CharacterRenderSystem();

            Entities = EntityManager.Create("LocalEntities",
                new List<BaseSystem> {
                    // Input
                    new InputSystem(),

                    // Movement
                    new EntityCollidingMover(),
                    new BlockCollidingMoverSystem(),
                    new MoverSystem(), // <- Last

                    // Collision Resolution
                    new CollisionApplicatorSystem(),
                    new EntityCollisionRemoverSystem(), // <- Last

                    new TimeDamageSystem(),
                    // Renderer
                    CharacterRender,
                }
            );

            
        }

        public void StartMap(Map m)
        {
            World = new World(Entities);
            World.ChangeMap(Game.TestMap);
            Game.Player.PlayerId = World.CreatePlayer(Game.Player.Input, Game.Player.Position);

        }

        public override void Update(engenious.GameTime gameTime)
        {
            base.Update(gameTime);

            if(World != null && World.CurrentMap != null)
                World.Update(new Core.GameTime(gameTime.ElapsedGameTime,gameTime.TotalGameTime));
        }
    }
}
