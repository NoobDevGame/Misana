using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using engenious;
using Misana.Core;
using Misana.Core.Client;
using Misana.Core.Communication;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;
using Misana.Core.Network;
using Misana.Core.Server;
using Misana.EntityComponents;
using Misana.EntitySystems;
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
        private IServerOnClient networkClient;
        private List<BaseSystem> renderSystems;

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

            SpriteRenderSystem = new SpriteRenderSystem(Game);
            //SpriteRenderSystem.LoadContent();

            HealthRenderSystem = new HealthRenderSystem(Game);
            //HealthRenderSystem.LoadContent();

            NameRenderSystem = new NameRenderSystem(Game);
            //NameRenderSystem.LoadContent();



            renderSystems = new List<BaseSystem>();
            renderSystems.Add(SpriteRenderSystem);
            renderSystems.Add(HealthRenderSystem);
            renderSystems.Add(NameRenderSystem);

            NetworkManager.SetPorts(localUdpPort:NetworkManager.StandardPort +1);

            serverHost = new ServerGameHost();
        }
        
        public async Task StartLocalGame(Map map)
        {
            await CreateLocalServer("Asdf");
            await CreateWorld("LocalWorld", map);
            await StartWorld();
        }

        public async Task CreateLocalServer(string localplayer)
        {
            serverHost.StartListening();
            host = new ClientGameHost(null,renderSystems);
            var c = serverHost.CreateLocalClient(host);
            networkClient = c.ServerOnClient;
            host.Server = this.networkClient;
            networkClient.ClientRpcHandler = host;

            host.SimulationStarted += OnSimulationStarted;
            host.WorldInfoReceived += OnWorldInfoReceived;
            host.PlayerInfoReceived += OnPlayerInfoReceived;
            host.InitialGameStateReceived += OnInitialGameState;

            var id = await host.Connect("Test",IPAddress.Loopback);
            LocalPlayerInfo = new PlayerInfo("Test",id);
        }



        public async Task ConnectToServer(string name, IPAddress address)
        {
            networkClient = new ServerOnClient();
            host = new ClientGameHost(null,renderSystems);
            host.Server = this.networkClient;
            networkClient.ClientRpcHandler = host;

            host.SimulationStarted += OnSimulationStarted;
            host.WorldInfoReceived += OnWorldInfoReceived;
            host.PlayerInfoReceived += OnPlayerInfoReceived;
            host.InitialGameStateReceived += OnInitialGameState;

            var id = await host.Connect(name,address);
            LocalPlayerInfo = new PlayerInfo(name,id);
        }

        private void OnWorldInfoReceived(WorldInformation obj)
        {
            lock(WorldInformations)
                WorldInformations.Add(obj);
        }

        private void OnPlayerInfoReceived(PlayerInfo obj)
        {
            Players.Add(obj);
        }

        private void OnSimulationStarted()
        {

        }
        public bool Joined = true;

        public async Task CreateWorld(string name, Map map)
        {
            Joined = false;
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
           // Game.Player.PlayerId = await host.CreatePlayer(Game.Player.Input, Game.Player.Transform);
            await Simulation.Start();
        }

        private void OnInitialGameState(InitialGameState obj)
        {
            host.Entities.Clear();

            var playerEntity = obj.Entities.First(x => x.Id == obj.PlayerId);

            ComponentRegistry.Copy[ComponentRegistry<TransformComponent>.Index](playerEntity.Get<TransformComponent>(), Game.Player.Transform);
            ComponentRegistry<TransformComponent>.Release(playerEntity.Get<TransformComponent>());
            playerEntity.Components[ComponentRegistry<TransformComponent>.Index] = Game.Player.Transform;
            playerEntity.Components[ComponentRegistry<PlayerInputComponent>.Index] = Game.Player.Input;
            playerEntity.Components[ComponentRegistry<SendComponent>.Index] = ComponentRegistry<SendComponent>.Take();

            foreach (var e in obj.Entities)
            {
                e.Manager = host.Entities;

                var eSend = e.Get<SendComponent>();
                if (e != playerEntity && eSend != null)
                {
                    ComponentRegistry<SendComponent>.Release(eSend);
                    e.Components[ComponentRegistry<SendComponent>.Index] = null;
                }

                host.Entities.AddEntity(e);
            }

            Game.Player.PlayerId = obj.PlayerId;


            if(Joined)
                Simulation.Start();


        }

        public override void Update(GameTime gameTime)
        {
            if (host?.Server == null)
                return;
            
            base.Update(gameTime);

            host.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
            serverHost.Update(new Core.GameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime));
        }

        public void Close()
        {
            serverHost.StopListening();
        }

        public async Task JoinWorld(WorldInformation worldListSelectedItem)
        {
            await host.JoinWorld(worldListSelectedItem.Id);
            AddHooks();
            Players.Add(LocalPlayerInfo);

        }
    }
}
