using engenious;
using engenious.Graphics;
using Misana.Components;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.EntityComponents;

namespace Misana.EntitySystems
{
    internal class HealthRenderSystem : BaseSystemR3<RenderHealthComponent, HealthComponent, RenderSpriteComponent>
    {
        private readonly MisanaGame _game;
        private CameraComponent _camera;
        private Texture2D _pixel;

        internal HealthRenderSystem(MisanaGame game)
        {
            _game = game;
            _camera = _game.CameraComponent;
        }

        public void LoadContent()
        {
            _pixel = new Texture2D(_game.GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });
        }

        public void Draw(MisanaGame game, GameTime gameTime, SpriteBatch batch)
        {
            for (int i = 0; i < Count; i++)
            {
                var item = R1S[i];
                if (item.Render)
                {
                    batch.Draw(_pixel,item.Background,Color.LightGray);
                    batch.Draw(_pixel,item.FillRect, Color.Red);
                }
            }
        }

        protected override void Update(Entity e, RenderHealthComponent r1, HealthComponent r2, RenderSpriteComponent r3)
        {
            r1.Render = false;

            if(!r3.Render)
                return;

            if(r2.Current >= r2.Max)
                return;


            if(engenious.Vector2.Distance(r3.Center, _camera.PlayerPosition) > 5 * 32 * _camera.Zoom)
                return;

            var len = 50 * _camera.Zoom;

            var pos = r3.Center + new Vector2(-len / 2f, - (r3.Destination.Width / 2) - 5);
            r1.Background = new Rectangle(
                (int) pos.X,
                (int) pos.Y,
                (int) (len),
                (int) (5 * _camera.Zoom)
            );

            r1.FillRect = new Rectangle(
                (int) pos.X,
                (int) pos.Y,
                (int) (len * r2.Ratio),
                (int) (5 * _camera.Zoom)
            );

            r1.Render = true;
        }
    }
}