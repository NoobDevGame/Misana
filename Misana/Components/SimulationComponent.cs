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

        private ClientGameHost host;
        private ServerGameHost serverHost;
        private NetworkClient networkClient;

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

            networkClient = new NetworkClient();

            System.Collections.Generic.List<BaseSystem> renderSystems = new System.Collections.Generic.List<BaseSystem>();
            renderSystems.Add(SpriteRenderSystem);
            renderSystems.Add(HealthRenderSystem);
            renderSystems.Add(NameRenderSystem);

            host = new ClientGameHost(networkClient, null,renderSystems);
            serverHost = new ServerGameHost(networkClient.Outer);
        }

        public async Task StartLocalGame(Map map)
        {
            await CreateWorld("LcoalWorld",map);
            await StartWorld();
        }

        public async Task ConnectToServer(string name)
        {
            await host.Connect(name);
        }

        public async Task CreateWorld(string name, Map map)
        {
            Simulation = await host.CreateWorld(name);

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

            await Simulation.ChangeMap(map);
        }

        public async Task StartWorld()
        {
            Game.Player.PlayerId = await Simulation.CreatePlayer(Game.Player.Input, Game.Player.Transform);
            await Simulation.Start();
        }

        public override void Update(engenious.GameTime gameTime)
        {
            base.Update(gameTime);

            host.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
            serverHost.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
        }
    }
}
