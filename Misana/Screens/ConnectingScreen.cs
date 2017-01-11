using Misana.Components;
using MonoGameUi;

namespace Misana.Screens
{
    internal class ConnectingScreen : Screen
    {
        Label infoLabel;

        ScreenComponent Manager;

        public ConnectingScreen(ScreenComponent manager, bool error = false, string message = "Connecting..." ) : base(manager)
        {
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
