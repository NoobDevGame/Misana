using System.Threading;
using System.Threading.Tasks;
using engenious;
using Misana.Components;
using MonoGameUi;

namespace Misana.Screens
{
    internal class ConnectingScreen : Screen
    {
        Label infoLabel;

        ScreenComponent Manager;

        private Task waitTask;

        public ConnectingScreen(ScreenComponent manager,Task waitTask, bool error = false, string message = "Connecting..." ) : base(manager)
        {
            this.waitTask = waitTask;
            Manager = manager;

            infoLabel = new Label(manager);
            infoLabel.Text = message;
            Controls.Add(infoLabel);

            if (error)
                ConnectionError();
            else
                Connect();
        }

        private void Connect()
        {
            //Connection here
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (waitTask.IsCompleted)
            {
                Manager.NavigateToScreen(new GameScreen(Manager));
            }
        }

        private void ConnectionError()
        {
            Button backButton = Button.TextButton(Manager, "Back");
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.Margin = new Border(10, 10, 10, 10);
            backButton.LeftMouseClick += (s, e) => { Manager.NavigateBack(); };
            Controls.Add(backButton);
        }


    }
}
