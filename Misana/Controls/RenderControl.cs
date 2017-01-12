﻿using System;
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

        private Texture2D _pixel;
        private IndexBuffer ib;
        private Effect effect;
        private AreaRenderer _renderer;

        public Dictionary<string, Tilesheet> Tilesheets { get; private set; }

        public RenderControl(ScreenComponent manager, string style = "") : base(manager, style)
        {
            this.manager = manager;
            _pixel = new Texture2D(manager.GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });
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
            world.M11 = world.M22=world.M33 = manager.Game.CameraComponent.TileSize;
            effect.Parameters["WorldViewProj"].SetValue(projection*world);
            //effect.Parameters["offset"].SetValue(new Vector2(cOffset.X+player.Radius * 70, cOffset.Y+player.Height * 70));
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



            var player = manager.Game.Player.Player;
            var area = manager.Game.TestMap.StartArea;

            if (area == null)
                return;

            if (player == null)
                return;

            if (ControlTexture != null)
            {
                batch.Draw(ControlTexture,new Vector2(0,0),Color.White);
            }

            var camera = manager.Game.CameraComponent;

            var dimensioncomp = player.Get<DimensionComponent>();
            var positioncomp = player.Get<PositionComponent>();

            Vector2 dimension = new Vector2(1, 1);

            if (dimensioncomp != null)
                dimension = new Vector2(dimensioncomp.HalfSize.X, dimensioncomp.HalfSize.Y);

            dimension *= camera.TileSize;

            var position = camera.PlayerPosition - dimension;

            batch.Draw(_pixel, new Rectangle((int)position.X,(int)position.Y,(int)(dimension.X * 2), (int)(dimension.Y * 2)), Color.Red);


            /*
            foreach (var entity in area.Entities)
            {
                Vector2 position = new Vector2((entity.Position.X) * 70 + cOffset.X, (entity.Position.Y) * 70 + cOffset.Y  );

                if (entity == manager.Game.SimulationComponent.Player)
                {
                    position = manager.Game.CameraComponent.HalfViewport - new Vector2(entity.Radius, entity.Height);
                }
                var pathName = entity.TextureName;
                if (entity is ICharacter)
                    pathName = Path.Combine("player" , pathName);
                Rectangle destination = new Rectangle((int)(position.X), (int)(position.Y), (int)(entity.Radius * 2 * 70), (int)(entity.Height * 70));
                batch.Draw(manager.Content.Load<Texture2D>(pathName), destination, Color.White);
            }
            */

        }

        public void Dispose()
        {
            ib?.Dispose();
        }
    }
}
