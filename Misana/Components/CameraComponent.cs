using engenious;
using Misana.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Components
{
    internal class CameraComponent : DrawableGameComponent
    {
        public new MisanaGame Game;

        public Vector2 CameraOffset { get; set; }

        public Vector2 PlayerPosition { get; set; }

        public readonly int TileSize = 32;

        public int Zoom { get; set; }

        public CameraComponent(MisanaGame game) : base(game)
        {
            Game = game;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Game.Player.Player == null)
                return;

            var position = Game.Player.Player.Get<PositionComponent>();

            Vector2 viewportHalf = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) / 2;

            CameraOffset = viewportHalf - new Vector2(position.Position.X * TileSize, position.Position.Y * TileSize);

            PlayerPosition = viewportHalf;

        }
    }
}
