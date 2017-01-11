using engenious;
using engenious.Input;
using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Components
{
    internal class PlayerGameComponent : GameComponent
    {
        public Entity Player { get; set; }

        public new MisanaGame Game;

        public PlayerGameComponent(MisanaGame game) : base(game)
        {
            Game = game;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Player == null)
                return;

            var input = Player.Get<PlayerInputComponent>();

            Misana.Core.Vector2 move = Misana.Core.Vector2.Zero;

            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.A))
                move += new Misana.Core.Vector2(-1, 0);

            if (keyboard.IsKeyDown(Keys.D))
                move += new Misana.Core.Vector2(1, 0);

            if (keyboard.IsKeyDown(Keys.W))
                move += new Misana.Core.Vector2(0, -1);

            if (keyboard.IsKeyDown(Keys.S))
                move += new Misana.Core.Vector2(0, 1);

            input.Move = move;
        }
    }
}
