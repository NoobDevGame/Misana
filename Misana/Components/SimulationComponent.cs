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
using Misana.Network;

namespace Misana.Components
{
    internal class SimulationComponent : GameComponent
    {
        public ISimulation Simulation { get; private set; }

        public new MisanaGame Game;
        
        public SpriteRenderSystem SpriteRenderSystem;
        public HealthRenderSystem HealthRenderSystem;
        public NameRenderSystem NameRenderSystem;

        private GameHost host;
        private GameHost serverHost;
        private InternNetworkClient networkClient;

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

            networkClient = new InternNetworkClient();

            List<BaseSystem> renderSystems = new List<BaseSystem>();
            renderSystems.Add(SpriteRenderSystem);
            renderSystems.Add(HealthRenderSystem);
            renderSystems.Add(NameRenderSystem);

            host = new GameHost(GameHostMode.Local,networkClient, null,renderSystems);
            serverHost = new GameHost(GameHostMode.Server, networkClient.Server,null,null);
        }

        public void StartLocalGame(Map m)
        {
            host.Connect();

            Simulation = host.CreateWorld("LocalWorld");

            Simulation.Entities.RegisterAdditionHook<SpriteInfoComponent>(
                (em, e, si) => {
                    var rsc = ComponentRegistry<RenderSpriteComponent>.Take();
                    em.Add(e, rsc, false);
                }
            );

            Simulation.Entities.RegisterRemovalHook<SpriteInfoComponent>(
                (em,e,si) => e.Remove<RenderSpriteComponent>()
            );

            Simulation.Entities.RegisterAdditionHook<CharacterComponent>(
                (em, e, si) => {
                    var rnc = ComponentRegistry<RenderNameComponent>.Take();
                    rnc.Text = si.Name;
                    em.Add(e, rnc, false);
                }
            );

            Simulation.Entities.RegisterRemovalHook<RenderNameComponent>(
                (em, e, si) => e.Remove<RenderNameComponent>()
            );

            Simulation.Entities.RegisterAdditionHook<HealthComponent>(
                (em, e, si) => {
                    em.Add(e, ComponentRegistry<RenderHealthComponent>.Take(), false);
                }
            );

            Simulation.Entities.RegisterRemovalHook<HealthComponent>(
               (em, e, si) => e.Remove<RenderHealthComponent>()
           );
            
            Simulation.ChangeMap(m);

            Game.Player.PlayerId = Simulation.CreatePlayer(Game.Player.Input, Game.Player.Transform);

        }

        public override void Update(engenious.GameTime gameTime)
        {
            base.Update(gameTime);

            host.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
            serverHost.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
        }
    }
}
