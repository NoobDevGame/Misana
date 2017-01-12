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
                                new InputSystem(),
                                new EntityCollidingMover(),
                                new BlockCollidingMoverSystem(),
                                new NonCollidingMoverSystem(),
                                CharacterRender,
                }
            );

            World = new World(Entities);
            World.ChangeMap(Game.TestMap);

            Game.Player.PlayerId =  World.CreatePlayer(Game.Player.Input,Game.Player.Position);
        }

        public override void Update(engenious.GameTime gameTime)
        {
            base.Update(gameTime);

            World.Update(new Core.GameTime(gameTime.ElapsedGameTime,gameTime.TotalGameTime));
        }
    }
}
