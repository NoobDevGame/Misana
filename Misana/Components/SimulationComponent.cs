using engenious;
using Misana.Controls;
using Misana.Core;
using Misana.Core.Ecs;
using Misana.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Systems.StatusSystem;
using Misana.Core.Maps;

namespace Misana.Components
{
    internal class SimulationComponent : GameComponent
    {
        public World World { get; private set; }

        public new MisanaGame Game;

        public CharacterRenderSystem CharacterRender { get; private set; }
        public SimulationComponent(MisanaGame game) : base(game)
        {
            Game = game;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            var foo = EntityManager.ComponentCount;

            CharacterRender = new CharacterRenderSystem();
        }

        public void StartMap(Map m)
        {
            List<BaseSystem> renderSystems = new List<BaseSystem>();
            renderSystems.Add(CharacterRender);

            World = new World(renderSystems);
            World.ChangeMap(m);

            Game.Player.PlayerId = World.CreatePlayer(Game.Player.Input, Game.Player.Transform);

        }

        public override void Update(engenious.GameTime gameTime)
        {
            base.Update(gameTime);

            if(World != null && World.CurrentMap != null)
                World.Update(new Core.GameTime(gameTime.ElapsedGameTime,gameTime.TotalGameTime));
        }
    }
}
