using engenious;
using Misana.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace Misana.Components
{
    internal class CameraComponent : DrawableGameComponent
    {
        public new MisanaGame Game;

        public Vector2 CameraOffset { get; set; }

        public Vector2 PlayerPosition { get; set; }

        public readonly int TileSize = 32;

        public float Zoom { get; set; }

        public CameraComponent(MisanaGame game) : base(game)
        {
            Game = game;
            Zoom = 0.5f;
        }

        private float _lastScroll = 0;
        private float _scrollValue = 0;
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var ms = engenious.Input.Mouse.GetState();

            _scrollValue += ms.ScrollWheelValue - _lastScroll;
            if (_scrollValue < 0)
                _scrollValue = 0;
            else if (_scrollValue > 120)
                _scrollValue = 120;

            var value = (_scrollValue / 120f);

            _lastScroll = ms.ScrollWheelValue;

            Zoom = ((float) Math.Cos(value * MathHelper.Pi +  MathHelper.PiOver2) + 1.1f) * 4f;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var position = Game.Player.Transform;

            Vector2 viewportHalf = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) / 2;

            CameraOffset = viewportHalf - new Vector2(position.Position.X * TileSize, position.Position.Y * TileSize) * Zoom;

            PlayerPosition = viewportHalf;

        }

        public Vector2 ViewToWorld(Vector2 viewVector)
        {
            return PlayerPosition - viewVector;
        }
    }
}
