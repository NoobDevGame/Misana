using engenious;
using engenious.Graphics;
using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Controls
{
    internal class CharacterRenderSystem : BaseSystemR2O1<CharacterRenderComponent,PositionComponent,DimensionComponent>
    {
        private Texture2D _pixel;
        private Texture2D _characterTexture;

        protected override void Update(Entity e, CharacterRenderComponent r1, PositionComponent r2, DimensionComponent o2)
        {
        }

        public void LoadContent(MisanaGame game)
        {
            _pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });

            _characterTexture = game.Content.Load<Texture2D>("TileSheetCharacters");
        }

        public void Draw(MisanaGame game,GameTime gameTime,SpriteBatch batch)
        {
            var area = game.Player.Position.CurrentArea;
            var camera = game.CameraComponent;


            for (int i = 0; i < Count; i++)
            {
                var renderComponent = R1S[i];
                var positionComponent = R2S[i];
                var dimensionComponent = O1S[i];

                if (positionComponent.CurrentArea.Id != area.Id)
                    continue;

                var entity = Entities[i];

                Vector2 dimension = new Vector2(0.5f, 0.5f);
                if (dimensionComponent != null)
                    dimension = new Vector2(dimensionComponent.HalfSize.X, dimensionComponent.HalfSize.Y);

                dimension *= camera.TileSize;

                var source = new Rectangle(renderComponent.TilePosition.X * 17, renderComponent.TilePosition.Y * 17, 16, 16);

                Vector2 position = new Vector2(positionComponent.Position.X, positionComponent.Position.Y) * camera.TileSize + camera.CameraOffset ;

                if (entity.Id == game.Player.PlayerId)
                    position = camera.PlayerPosition;

                position -= dimension;
                
                

                batch.Draw(_characterTexture, new Rectangle((int)position.X, (int)position.Y, (int)(dimension.X * 2 * camera.Zoom), (int)(dimension.Y * 2 * camera.Zoom)),source, Color.White);
            }
        }
    }
}
