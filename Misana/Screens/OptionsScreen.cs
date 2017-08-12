using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using engenious.Graphics;
using Misana.Components;
using MonoGameUi;

namespace Misana.Screens
{
    internal class OptionsScreen : Screen
    {
        public class PlayerTexture
        {
            public string TextureName { get; set; }
            public Texture2D Texture { get; set; }

            public PlayerTexture(string name, Texture2D texture)
            {
                TextureName = name;
                Texture = texture;
            }
        }

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            StackPanel mainStack = new StackPanel(manager);
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(mainStack);

            mainStack.Controls.Add(new Panel(manager) { Height = 10, Width = 10 });

            mainStack.Controls.Add(new Label(manager) { Text = "Username: ", HorizontalAlignment = HorizontalAlignment.Left });

            Textbox nameInput = new Textbox(manager);
            nameInput.HorizontalAlignment = HorizontalAlignment.Stretch;
            nameInput.Margin = new Border(0, 0, 0, 10);
            mainStack.Controls.Add(nameInput);

            mainStack.Controls.Add(new Panel(manager) { Height = 10, Width = 10 });

            Button saveButton = Button.TextButton(manager, "Save");
            saveButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            saveButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            mainStack.Controls.Add(saveButton);

        }
    }
}
