using engenious;
using Misana.Core;
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

        public SimulationComponent(MisanaGame game) : base(game)
        {
            Game = game;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            World = new World();
            World.ChangeMap(Game.TestMap);

            Game.Player.Player = World.CreatePlayer();
        }

        public override void Update(engenious.GameTime gameTime)
        {
            base.Update(gameTime);

            World.Update(new Core.GameTime(gameTime.ElapsedGameTime,gameTime.TotalGameTime));
        }
    }
}
