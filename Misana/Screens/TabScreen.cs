using engenious;
using Misana.Components;
using MonoGameUi;

namespace Misana.Screens
{
    class TabScreen : Screen
    {
        public TabScreen(ScreenComponent manager) : base(manager)
        {
            IsOverlay = true;

            StackPanel panel = new StackPanel(manager);
            panel.MinHeight = 200;
            panel.MinWidth = 200;
            panel.Background = new BorderBrush(Color.Black * 0.3f);
            Controls.Add(panel);

            /*
            foreach(var player in manager.Game.SimulationComponent.Simulation.Players)
            {
                panel.Controls.Add(new Label(manager) { Text = player.Name, VerticalAlignment = VerticalAlignment.Top });
            }
            */
        }
    }
}
