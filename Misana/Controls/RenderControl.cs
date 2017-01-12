using System;
using System.Collections.Generic;
using System.IO;
using engenious;
using engenious.Graphics;
using Misana.Components;
using MonoGameUi;
using Misana.Core.Components;
using Misana.Core.Maps;

namespace Misana.Controls
{
    internal class RenderControl : Control,IDisposable
    {
        ScreenComponent manager;

        private IndexBuffer ib;
        private Effect effect;
        private AreaRenderer _renderer;

        public Dictionary<string, Tilesheet> Tilesheets { get; private set; }

        public RenderControl(ScreenComponent manager, string style = "") : base(manager, style)
        {
            this.manager = manager;
            

            manager.Game.Simulation.CharacterRender.LoadContent(manager.Game);

            effect = manager.Content.Load<Effect>("simple");
            CreateIndexBuffer();
            LoadTilesheets();

        }

        public void LoadTilesheets()
        {
            Tilesheets = new Dictionary<string, Tilesheet>();
            foreach (var tf in Directory.GetFiles("Content/Tilesheets/", "*.json"))
            {
                try
                {
                    var ts = Tilesheet.LoadTilesheet("Content/Tilesheets/", Path.GetFileNameWithoutExtension(tf));
                    Tilesheets.Add(ts.Name, ts);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not load tilesheet " + Path.GetFileNameWithoutExtension(tf));
                }
            }
        }

        public override void OnResolutionChanged()
        {
            base.OnResolutionChanged();
            if (ControlTexture != null)
{
                ControlTexture.Dispose();
                ControlTexture = null;
            }
        }

        private void CreateIndexBuffer()
        {
            List<uint> indices = new List<uint>(256*256*6);
            for (uint i = 0; i < 256*256*4; i+=4)
            {
                indices.Add(0+i);
                indices.Add(1+i);
                indices.Add(3+i);

                indices.Add(0+i);
                indices.Add(3+i);
                indices.Add(2+i);
            }
            ib = new IndexBuffer(manager.GraphicsDevice,DrawElementsType.UnsignedInt,indices.Count);
            ib.SetData(indices.ToArray());
        }
        public RenderTarget2D ControlTexture { get; set; }
        protected override void OnPreDraw(GameTime gameTime)
        {
            if (ActualClientArea.Width == 0 || ActualClientArea.Height == 0)
                return;
            if (ControlTexture == null)
            {
                ControlTexture = new RenderTarget2D(manager.GraphicsDevice, ActualClientArea.Width, ActualClientArea.Height, PixelInternalFormat.Rgb8);
            }

            var area = manager.Game.TestMap.StartArea;

            if (area == null)
                return;

            var cameraOffset = manager.Game.CameraComponent.CameraOffset;

            manager.GraphicsDevice.SetRenderTarget(ControlTexture);
            manager.GraphicsDevice.Clear(Color.Black);
            var cOffset = Vector2.Zero;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, manager.GraphicsDevice.Viewport.Width, 0, manager.GraphicsDevice.Viewport.Height, 0, -1);
            Matrix view = Matrix.CreateLookAt(new Vector3(0,0,1),Vector3.Zero,Vector3.UnitY);
            Matrix world =Matrix.CreateTranslation(cameraOffset.X,cameraOffset.Y,0);
            world.M11 = world.M22=world.M33 = manager.Game.CameraComponent.TileSize*manager.Game.CameraComponent.Zoom;
            effect.Parameters["WorldViewProj"].SetValue(projection *world);

            if (_renderer?.Area != area)
            {
                _renderer?.Dispose();
                _renderer = new AreaRenderer(manager,area, Tilesheets);
            }
            manager.GraphicsDevice.IndexBuffer = ib;
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _renderer.Render(effect);
            }
            manager.GraphicsDevice.SetRenderTarget(null);
        }

        protected override void OnDraw(SpriteBatch batch, Rectangle controlArea, GameTime gameTime)
        {
            base.OnDraw(batch, controlArea, gameTime);

            var area = manager.Game.Player.Position.CurrentArea;

            if (area == null)
                return;

            if (ControlTexture != null)
            {
                batch.Draw(ControlTexture,new Vector2(0,0),Color.White);
            }

            manager.Game.Simulation.CharacterRender.Draw(manager.Game,gameTime, batch);
        }

        public void Dispose()
        {
            ib?.Dispose();
        }
    }
}
