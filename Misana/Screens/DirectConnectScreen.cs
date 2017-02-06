using System;
using System.Net;
using Misana.Components;
using MonoGameUi;
using NoobFight.Screens;

namespace Misana.Screens
{
    internal class DirectConnectScreen : Screen
    {
        public DirectConnectScreen(ScreenComponent manager) : base(manager)
        {
            Padding = new Border(0, 0, 0, 0);

            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            stack.Controls.Add(new Label(manager) { Text = "Outer Address: ", HorizontalAlignment = HorizontalAlignment.Left });

            Textbox ipInput = new Textbox(manager);
            ipInput.HorizontalAlignment = HorizontalAlignment.Stretch;
            ipInput.Margin = new Border(0, 0, 0, 10);
            //ipInput.Text = "93.218.154.206";
            stack.Controls.Add(ipInput);

            stack.Controls.Add(new Panel(manager) { Height = 10, Width = 10 });

            Button connectButton = Button.TextButton(manager, "Connect");
            connectButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            connectButton.Margin = new Border(0, 0, 0, 10);
            connectButton.MinWidth = 300;
            connectButton.LeftMouseClick += (s, e) =>
            {
                try
                {
                    //TODO:Nickname ändern
                    var splt = ipInput.Text.Split(':');
                    int port = 4344;
                    if (splt.Length > 1)
                        int.TryParse(splt[1], out port);

                    var ip = string.IsNullOrEmpty(splt[0]) ? IPAddress.Loopback : IPAddress.Parse(splt[0]);

                    var task = manager.Game.Simulation.ConnectToServer("LocalPlayer",ip);
                    manager.NavigateToScreen(new ConnectingScreen(manager,task, (m) => new WorldSelectScreen(m)));

                    //manager.Game.NetworkComponent.Connect(splt[0], port,manager.Game.PlayerComponent.PlayerName,manager.Game.PlayerComponent.PlayerTexture);
                }
                catch (Exception ex)
                {
                    //manager.NavigateToScreen(new ConnectingScreen(manager, true, "Connection Error"));
                }
            };
            stack.Controls.Add(connectButton);

            Button backButton = Button.TextButton(manager, "Back");
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.Margin = new Border(10, 10, 10, 10);
            backButton.LeftMouseClick += (s, e) => { manager.NavigateBack(); };
            Controls.Add(backButton);
        }
    }
}
