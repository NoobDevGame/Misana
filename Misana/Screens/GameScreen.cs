using engenious.Graphics;
using engenious.Input;
using Misana.Components;
using Misana.Controls;
using Misana.Core;
using MonoGameUi;
using NoobFight.Screens;
using Vector2 = Misana.Core.Vector2;

namespace Misana.Screens
{
    internal class GameScreen : Screen
    {
        new ScreenComponent Manager;

        public GameScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;

            Padding = new Border(0, 0, 0, 0);

            RenderControl renderControl = new RenderControl(manager);
            renderControl.VerticalAlignment = VerticalAlignment.Stretch;
            renderControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(renderControl);

        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key == Keys.Tab)
                Manager.NavigateToScreen(new TabScreen(Manager));
        }

        protected override void OnKeyUp(KeyEventArgs args)
        {
            base.OnKeyUp(args);

            if (Manager.ActiveScreen is TabScreen)
                Manager.NavigateBack();
        }

        protected override void OnUpdate(engenious.GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Manager.ActiveScreen is PauseScreen || Manager.ActiveScreen is TabScreen)
                return;

            /*

            var key = Keyboard.GetState();

            if (key.IsKeyDown(Keys.A))
                input.MoveLeft = true;

            if (key.IsKeyDown(Keys.W))
                input.Jump = true;

            if (key.IsKeyDown(Keys.D))
                input.MoveRight = true;

            if (key.IsKeyDown(Keys.Space))
                input.Jump = true;

            if (key.IsKeyDown(Keys.Escape))
                Manager.NavigateToScreen(new PauseScreen(Manager));

            var mouse = Mouse.GetState();
            if (mouse.X < Manager.Game.Window.ClientRectangle.Width && mouse.Y < Manager.Game.Window.ClientRectangle.Height)
            {
                if (mouse.IsButtonDown(MouseButton.Left))
                    input.LeftClick = true;

                if (mouse.IsButtonDown(MouseButton.Right))
                    input.RightClick = true;
                
                input.MousePosition = new Vector2(mouse.X - Manager.Game.Window.ClientRectangle.Width/2, mouse.Y- Manager.Game.Window.ClientRectangle.Height/2);
            }

            */
        }
    }
}
