using engenious;
using engenious.Graphics;
using Misana.Components;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.EntityComponents;

namespace Misana.EntitySystems
{
    internal class SpriteRenderSystem : BaseSystemR3<RenderSpriteComponent, SpriteInfoComponent, TransformComponent>
    {
        private readonly MisanaGame _game;
        private CameraComponent _camera;
        private Texture2D _characterTexture;
        

        internal SpriteRenderSystem(MisanaGame game)
        {
            _game = game;
            _camera = _game.CameraComponent;
        }

        public void LoadContent()
        {
            _characterTexture = _game.Content.Load<Texture2D>("Tilesheets/TileSheetCharacters");
            _characterTexture.SamplerState = new SamplerState() { TextureFilter = TextureFilter.Nearest };
        }

        public void Draw(MisanaGame game, GameTime gameTime, SpriteBatch batch)
        {
            for (int i = 0; i < Count; i++)
            {
                var item = R1S[i];
                if (item.Render)
                {
                    batch.Draw(item.Texture, item.Destination, item.Source, item.Color);
                }
            }
        }

        protected override void Update(Entity e, RenderSpriteComponent r1, SpriteInfoComponent r2, TransformComponent r3)
        {
            var camera = _camera;

            r1.Render = false;
            if (r3.CurrentAreaId != _game.Player.Transform.CurrentAreaId)
            {
                r1.Render = false;
                return;
            }
            
            if (e.Id == _game.Player.PlayerId)
            {
                r1.Center = camera.PlayerPosition;
            }

            else if (r3.ParentEntityId > 0)
            {
                var parent = Manager.GetEntityById(r3.ParentEntityId);
                if (parent == null)
                {
                    return;
                }

                var parentRender = parent.Get<RenderSpriteComponent>();
                if(parentRender == null)
                    return;

                if(parentRender.Center == Vector2.Zero)
                    return;

                r1.Center = parentRender.Center + (new Vector2(r3.Position.X, r3.Position.Y) * camera.TileSize * camera.Zoom);
            }
            else
            {
                r1.Center = new Vector2(r3.Position.X, r3.Position.Y) * camera.TileSize * camera.Zoom + camera.CameraOffset;
            }

            var dimension = new Vector2(r3.HalfSize.X, r3.HalfSize.Y) * camera.TileSize * camera.Zoom;
            

            if (r2.TilePosition != r1.TilePosition)
            {
                r1.TilePosition = r2.TilePosition;
                r1.Source = new Rectangle(r2.TilePosition.X * 17, r2.TilePosition.Y * 17, 16, 16);
            }

            r1.Color = Color.White;
            r1.Texture = _characterTexture;
            var position = r1.Center - dimension;

            r1.Destination = new Rectangle((int) position.X, (int) position.Y, (int) (dimension.X * 2), (int) (dimension.Y * 2));
            r1.Render = true;
        }
    }
}