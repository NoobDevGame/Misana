using engenious;
using Misana.Components;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using MonoGameUi;
using NoobFight.Screens;

namespace Misana.Screens
{
    internal class WorldSelectScreen : Screen
    {
        Listbox<WorldInformation> worldList;

        public new ScreenComponent Manager;

        public WorldSelectScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            Grid grid = new Grid(manager);
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Parts });
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Parts });
            grid.Rows.Add(new RowDefinition() { Height = 1, ResizeMode = ResizeMode.Parts });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto });
            Controls.Add(grid);

            worldList = new Listbox<WorldInformation>(manager);
            worldList.HorizontalAlignment = HorizontalAlignment.Stretch;
            worldList.VerticalAlignment = VerticalAlignment.Stretch;
            worldList.TemplateGenerator = (s) =>
            {
                Panel p = new Panel(manager);
                p.HorizontalAlignment = HorizontalAlignment.Stretch;

                p.Controls.Add(new Label(manager) { Text = s.Name });
                return p;
            };
            grid.AddControl(worldList, 0, 0, 2 ,1);

            worldList.SelectFirst();

            Button joinButton = Button.TextButton(manager, "Join");
            joinButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            joinButton.Height = 50;
            joinButton.MinWidth = 300;
            joinButton.LeftMouseClick += (s, e) =>
            {
                var task = manager.Game.Simulation.JoinWorld(worldList.SelectedItem);
                manager.NavigateToScreen(new ConnectingScreen(manager, task, m => new LobbyScreen(m)));
            };
            grid.AddControl(joinButton, 0, 1);

            Button createButton = Button.TextButton(manager, "Create World");
            createButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            createButton.Height = 50;
            createButton.MinWidth = 300;
            createButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new NewWorldScreen(manager));
            };
            grid.AddControl(createButton, 1, 1);

            /*
            manager.Game.NetworkComponent.MessageHandler.RegisterMessageHandler<NewWorldBroadcast>((c, m) =>
            {
                if (!worldList.Items.Contains(m.WorldName) && IsActiveScreen)
                    worldList.Items.Add(m.WorldName);
            });
            */
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            foreach (var worldinformation in Manager.Game.Simulation.WorldInformations )
            {
                if (!worldList.Items.Contains(worldinformation) && IsActiveScreen)
                    worldList.Items.Add(worldinformation);
            }
        }
    }
}
