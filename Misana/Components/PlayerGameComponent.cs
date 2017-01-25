using engenious;
using engenious.Input;
using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core;
using GameTime = engenious.GameTime;
using Vector2 = Misana.Core.Vector2;

namespace Misana.Components
{
    internal class PlayerGameComponent : GameComponent
    {
        public PlayerInputComponent Input { get; private set; }

        public TransformComponent Transform { get; private set; }

        public int PlayerId { get; set; }

        public new MisanaGame Game;

        public PlayerGameComponent(MisanaGame game) : base(game)
        {
            Game = game;
            Input = new PlayerInputComponent()
            {
                Unmanaged = true,
            };

            Transform = new TransformComponent()
            {
                Unmanaged = true,
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            Misana.Core.Vector2 move = Misana.Core.Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.A))
                move += new Misana.Core.Vector2(-1, 0);

            if (keyboard.IsKeyDown(Keys.D))
                move += new Misana.Core.Vector2(1, 0);

            if (keyboard.IsKeyDown(Keys.W))
                move += new Misana.Core.Vector2(0, -1);

            if (keyboard.IsKeyDown(Keys.S))
                move += new Misana.Core.Vector2(0, 1);


            Input.Interact = keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.E);
            Input.Drop = keyboard.IsKeyDown(Keys.Q);
            Input.PickUp = keyboard.IsKeyDown(Keys.F);

            Input.MousePosition =new Vector2(mouse.X,mouse.Y);

            Input.Move = move;

          
            var wf = Game.CameraComponent.ViewToWorld(new engenious.Vector2(mouse.X, mouse.Y));
            Input.Facing = new Misana.Core.Vector2(wf.X, wf.Y);

            Input.Attacking = mouse.LeftButton == ButtonState.Pressed;
        }
    }
}
