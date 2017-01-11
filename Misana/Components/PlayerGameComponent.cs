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
        public PlayerInputComponent Input { get; private set; }

        public PositionComponent Position { get; private set; }

        public int PlayerId { get; set; }

        public new MisanaGame Game;

        public PlayerGameComponent(MisanaGame game) : base(game)
        {
            Game = game;
            Input = new PlayerInputComponent()
            {
                Unmanaged = true,
            };

            Position = new PositionComponent()
            {
                Unmanaged = true,
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var keyboard = Keyboard.GetState();

            Misana.Core.Vector2 move = Misana.Core.Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.A))
                move += new Misana.Core.Vector2(-1, 0);

            if (keyboard.IsKeyDown(Keys.D))
                move += new Misana.Core.Vector2(1, 0);

            if (keyboard.IsKeyDown(Keys.W))
                move += new Misana.Core.Vector2(0, -1);

            if (keyboard.IsKeyDown(Keys.S))
                move += new Misana.Core.Vector2(0, 1);

            Input.Move = move;
        }
    }
}
