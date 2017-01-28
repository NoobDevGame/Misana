using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using engenious;
using Misana.Core;
using Misana.Core.Communication;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;
using Misana.EntityComponents;
using Misana.EntitySystems;
using Misana.Network;
using GameTime = engenious.GameTime;

namespace Misana.Components
{
    internal class SimulationComponent : GameComponent
    {
        public ISimulation Simulation => host;

        public new MisanaGame Game;
        
        public SpriteRenderSystem SpriteRenderSystem;
        public HealthRenderSystem HealthRenderSystem;
        public NameRenderSystem NameRenderSystem;

        private ClientGameHost host;
        private ServerGameHost serverHost;
        private NetworkClient networkClient;

        public List<WorldInformation> WorldInformations { get;} = new List<WorldInformation>();
        public List<PlayerInfo> Players { get;  } = new List<PlayerInfo>();

        public PlayerInfo LocalPlayerInfo { get; private set; }

        public SimulationState SimualtionState
		{
			get
			{
				if (Simulation == null) 
				{
					return SimulationState.Unloaded;
				}
				return Simulation.State;
			}
		}

        public bool CanStart { get; set; } = false;

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



            List<BaseSystem> renderSystems = new List<BaseSystem>();
            renderSystems.Add(SpriteRenderSystem);
            renderSystems.Add(HealthRenderSystem);
            renderSystems.Add(NameRenderSystem);

            serverHost = new ServerGameHost();


            networkClient = new NetworkClient();
            host = new ClientGameHost(networkClient, null,renderSystems);

        }



        public async Task StartLocalGame(Map map)
        {
            await CreateWorld("LcoalWorld",map);
            await StartWorld();
        }

        public async Task CreateLocalServer(string localplayer)
        {
            serverHost.Start();
            await ConnectToServer(localplayer, IPAddress.Loopback);
        }

        public async Task ConnectToServer(string name, IPAddress address)
        {
            var id = await host.Connect(name,address);
            LocalPlayerInfo = new PlayerInfo(name,id);
        }

        public async Task CreateWorld(string name, Map map)
        {
            await host.CreateWorld(name);

            AddHooks();

            Players.Add(LocalPlayerInfo);

            await Simulation.ChangeMap(map);

            CanStart = true;
        }

        private void AddHooks()
        {
            Simulation.Entities.RegisterAdditionHook<SpriteInfoComponent>(
                (em, e, si) =>
                {
                    var rsc = ComponentRegistry<RenderSpriteComponent>.Take();
                    em.Add(e, rsc, false);
                }
            );

            Simulation.Entities.RegisterRemovalHook<SpriteInfoComponent>(
                (em, e, si) => e.Remove<RenderSpriteComponent>()
            );

            Simulation.Entities.RegisterAdditionHook<CharacterComponent>(
                (em, e, si) =>
                {
                    var rnc = ComponentRegistry<RenderNameComponent>.Take();
                    rnc.Text = si.Name;
                    em.Add(e, rnc, false);
                }
            );

            Simulation.Entities.RegisterRemovalHook<RenderNameComponent>(
                (em, e, si) => e.Remove<RenderNameComponent>()
            );

            Simulation.Entities.RegisterAdditionHook<HealthComponent>(
                (em, e, si) => { em.Add(e, ComponentRegistry<RenderHealthComponent>.Take(), false); }
            );

            Simulation.Entities.RegisterRemovalHook<HealthComponent>(
                (em, e, si) => e.Remove<RenderHealthComponent>()
            );
        }

        public async Task StartWorld()
        {
            Game.Player.PlayerId = await host.CreatePlayer(Game.Player.Input, Game.Player.Transform);
            await Simulation.Start();
        }

        public override void Update(GameTime gameTime)
        {
            {
                WorldInformationMessage message;
                while (host.Receiver.TryGetMessage(out message))
                {
                    WorldInformations.Add(new WorldInformation(message));
                }
            }

            {
                OnJoinWorldMessage message;
                while (host.Receiver.TryGetMessage(out message))
                {
                    Players.Add(new PlayerInfo(message.Name,message.PlayerId));
                }
            }

            {
                PlayerInfoMessage message;
                while (host.Receiver.TryGetMessage(out message))
                {

                    Players.Add(new PlayerInfo(message.Name,message.PlayerId));
                }
            }

            {
                OnStartSimulationMessage message;
                while (host.Receiver.TryGetMessage(out message))
                {
                    StartWorld();
                }
            }


            base.Update(gameTime);

            host.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
            serverHost.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
        }

        public void Close()
        {
            serverHost.Stop();
        }


        public async Task JoinWorld(WorldInformation worldListSelectedItem)
        {
            await host.JoinWorld(worldListSelectedItem.Id);
            AddHooks();
            Players.Add(LocalPlayerInfo);
        }
    }
}
