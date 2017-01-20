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
using Misana.Core.Components;
using Misana.Core.Systems.StatusSystem;
using Misana.Core.Maps;
using Misana.EntityComponents;
using Misana.EntitySystems;

namespace Misana.Components
{
    internal class SimulationComponent : GameComponent
    {
        public World World { get; private set; }

        public new MisanaGame Game;
        
        public SpriteRenderSystem SpriteRenderSystem;
        public HealthRenderSystem HealthRenderSystem;
        public NameRenderSystem NameRenderSystem;

        public SimulationComponent(MisanaGame game) : base(game)
        {
            Game = game;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            var foo = EntityManager.ComponentCount;


            SpriteRenderSystem = new SpriteRenderSystem(Game);
            //SpriteRenderSystem.LoadContent();

            HealthRenderSystem = new HealthRenderSystem(Game);
            //HealthRenderSystem.LoadContent();

            NameRenderSystem = new NameRenderSystem(Game);
            //NameRenderSystem.LoadContent();

        }

        public void StartMap(Map m)
        {
            List<BaseSystem> renderSystems = new List<BaseSystem>();
            renderSystems.Add(SpriteRenderSystem);
            renderSystems.Add(HealthRenderSystem);
            renderSystems.Add(NameRenderSystem);
            
            World = new World(renderSystems,null);

            World.Entities.RegisterAdditionHook<SpriteInfoComponent>(
                (em, e, si) => {
                    var rsc = ComponentRegistry<RenderSpriteComponent>.Take();
                    em.Add(e, rsc, false);
                }
            );

            World.Entities.RegisterRemovalHook<SpriteInfoComponent>(
                (em,e,si) => e.Remove<RenderSpriteComponent>()
            );

            World.Entities.RegisterAdditionHook<CharacterComponent>(
                (em, e, si) => {
                    var rnc = ComponentRegistry<RenderNameComponent>.Take();
                    rnc.Text = si.Name;
                    em.Add(e, rnc, false);
                }
            );

            World.Entities.RegisterRemovalHook<RenderNameComponent>(
                (em, e, si) => e.Remove<RenderNameComponent>()
            );

            World.Entities.RegisterAdditionHook<HealthComponent>(
                (em, e, si) => {
                    em.Add(e, ComponentRegistry<RenderHealthComponent>.Take(), false);
                }
            );

            World.Entities.RegisterRemovalHook<HealthComponent>(
               (em, e, si) => e.Remove<RenderHealthComponent>()
           );
            
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
