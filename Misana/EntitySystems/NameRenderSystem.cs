using engenious;
using engenious.Graphics;
using Misana.Components;
using Misana.Core.Ecs;
using Misana.EntityComponents;

namespace Misana.EntitySystems
{
    internal class NameRenderSystem : BaseSystemR2O1<RenderNameComponent, RenderSpriteComponent, RenderHealthComponent>
    {
        private readonly MisanaGame _game;
        private CameraComponent _camera;
        private SpriteFont _font;

        internal NameRenderSystem(MisanaGame game)
        {
            _game = game;
            _camera = _game.CameraComponent;
        }

        public void LoadContent()
        {

            _font = _game.Content.Load<SpriteFont>("Hud");
        }

        public void Draw(MisanaGame game, GameTime gameTime, SpriteBatch batch)
        {
            for (int i = 0; i < Count; i++)
            {
                var item = R1S[i];
                if (item.Render)
                {
                    batch.DrawString(_font, item.Text, item.Position, Color.LightGray, 0, Vector2.Zero, Vector2.Zero);
                }
            }
        }

        protected override void Update(Entity e, RenderNameComponent r1,  RenderSpriteComponent r2, RenderHealthComponent o1)
        {
            r1.Render = false;

            if (!r2.Render)
                return;

            if (string.IsNullOrEmpty(r1.Text))
                return;

            var dist = engenious.Vector2.Distance(r2.Center, _camera.PlayerPosition);
            if (dist > 5 * 32 * _camera.Zoom)
                return;

            var length = _font.MeasureString(r1.Text);
            
            r1.Position = r2.Center + new Vector2(-length.X / 2f, -(r2.Destination.Height / 2) - length.Y - ((o1?.Render ?? false) ? 3 : 0));
            
            r1.Render = true;
        }
    }
}