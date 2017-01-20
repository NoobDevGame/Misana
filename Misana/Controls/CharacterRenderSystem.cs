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
    internal class CharacterRenderSystem : BaseSystemR2<CharacterRenderComponent, TransformComponent>
    {
        private Texture2D _pixel;
        private Texture2D _characterTexture;
        private SpriteFont _font;
        
        public override void Tick() { }

        public void LoadContent(MisanaGame game)
        {
            _pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });

            _font = game.Content.Load<SpriteFont>("Hud");

            _characterTexture = game.Content.Load<Texture2D>("Tilesheets/TileSheetCharacters");
            _characterTexture.SamplerState = new SamplerState() {TextureFilter = TextureFilter.Nearest};
        }

        public void Draw(MisanaGame game,GameTime gameTime,SpriteBatch batch)
        {
            var area = game.Player.Transform.CurrentArea;
            var camera = game.CameraComponent;


            for (int i = 0; i < Count; i++)
            {
                var renderComponent = R1S[i];
                var positionComponent = R2S[i];

                Vector2 dimension;
                Vector2 position;

                if (positionComponent.ParentEntityId != 0)
                {
                    var parent = Manager.GetEntityById(positionComponent.ParentEntityId);
                    if (parent == null)
                    {
                        continue;
                    }

                    int parentIdx;
                    if(!IndexMap.TryGetValue(parent, out parentIdx))
                        continue;

                    var parentPos = R2S[parentIdx];

                    if(parentPos.CurrentArea.Id != area.Id)
                        continue;

                    dimension = new Vector2(positionComponent.HalfSize.X, positionComponent.HalfSize.Y);

                    if (positionComponent.ParentEntityId == game.Player.PlayerId)
                    {

                        position = (new Vector2(parentPos.Position.X + positionComponent.Position.X, parentPos.Position.Y + positionComponent.Position.Y) *
                                    camera.TileSize * camera.Zoom + camera.CameraOffset);
                    }
                    else
                    {
                        position = (new Vector2(parentPos.Position.X + positionComponent.Position.X, parentPos.Position.Y + positionComponent.Position.Y) *
                                   camera.TileSize * camera.Zoom + camera.CameraOffset);
                    }
                }
                else
                {
                    if (positionComponent.CurrentArea.Id != area.Id)
                        continue;

                    dimension = new Vector2(positionComponent.HalfSize.X, positionComponent.HalfSize.Y);
                    position = (new Vector2(positionComponent.Position.X, positionComponent.Position.Y) * camera.TileSize * camera.Zoom + camera.CameraOffset);
                }

               

                var entity = Entities[i];

               // Vector2 dimension = new Vector2(positionComponent.HalfSize.X, positionComponent.HalfSize.Y);

                dimension *= camera.TileSize * camera.Zoom;

                var source = new Rectangle(renderComponent.TilePosition.X * 17, renderComponent.TilePosition.Y * 17, 16, 16);

             //   Vector2 position = (new Vector2(positionComponent.Position.X, positionComponent.Position.Y) * camera.TileSize * camera.Zoom + camera.CameraOffset) ;

                var health = entity.Get<HealthComponent>();
                if (entity.Id == game.Player.PlayerId)
                {
                    position = camera.PlayerPosition;
                    var drawHealth = health != null && health.Current < health.Max;
                    if (drawHealth)
                    {
                        var len = 50 * camera.Zoom; 

                        var pos = position + new Vector2(-len / 2f, -dimension.Y - 5);

                        batch.Draw(_pixel, new Rectangle(
                            (int)pos.X, (int)pos.Y,
                            (int)(len ), (int)(5 * camera.Zoom)
                        ), Color.LightGray);

                        batch.Draw(_pixel, new Rectangle(
                            (int)pos.X, (int)pos.Y,
                            (int)(len * health.Ratio ), (int)(5 * camera.Zoom)
                        ), Color.Red);
                    }
                }
                else
                {
                    var characterInfo = entity.Get<CharacterComponent>();

                    if (characterInfo != null)
                    {

                        var distance = (positionComponent.Position - game.Player.Transform.Position).LengthSquared();


                        var drawname = distance < 9;


                        var drawHealth = distance < 9 && health != null && health.Current < health.Max;

                        if (drawname && !string.IsNullOrEmpty(characterInfo?.Name))
                        {
                            var length = _font.MeasureString(characterInfo.Name);
                            var textposition = position + new Vector2(-length.X / 2f, -dimension.Y - length.Y - (drawHealth ? 3 : 0));

                            batch.DrawString(_font, characterInfo.Name, textposition, Color.LightGray);
                        }

                        if (drawHealth)
                        {
                            var len = 50 * camera.Zoom;

                            var pos = position + new Vector2(-len / 2f, -dimension.Y - 5);

                            batch.Draw(
                                _pixel,
                                new Rectangle(
                                    (int) pos.X,
                                    (int) pos.Y,
                                    (int) (len),
                                    (int) (5 * camera.Zoom)
                                ),
                                Color.LightGray);

                            batch.Draw(
                                _pixel,
                                new Rectangle(
                                    (int) pos.X,
                                    (int) pos.Y,
                                    (int) (len * health.Ratio),
                                    (int) (5 * camera.Zoom)
                                ),
                                Color.Red);
                        }
                    }
                }
                
                position -= dimension;
                batch.Draw(_characterTexture, new Rectangle((int)position.X, (int)position.Y, (int)(dimension.X * 2 ), (int)(dimension.Y * 2)),source, Color.White);
            }
        }
    }
}
