using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Components;
using Misana.Core.Maps;

namespace Misana
{
    internal class MisanaGame : Game
    {
        public Map TestMap { get; private set; }

        public ScreenComponent ScreenManager { get; private set; }

        public CameraComponent CameraComponent { get; set; }

        public SimulationComponent Simulation { get; private set; }

        public PlayerGameComponent Player { get; private set; }

        public MisanaGame()
        {

            Player = new PlayerGameComponent(this);
            Player.UpdateOrder = 1;
            Components.Add(Player);

            ScreenManager = new ScreenComponent(this);
            ScreenManager.UpdateOrder = 2;
            ScreenManager.DrawOrder = 2;

            Simulation = new SimulationComponent(this);
            Simulation.UpdateOrder = 3;
            Components.Add(Simulation);

            CameraComponent = new CameraComponent(this);
            CameraComponent.UpdateOrder = 4;
            CameraComponent.DrawOrder = 1;
            Components.Add(CameraComponent);

            Components.Add(ScreenManager);
        }

        public override void LoadContent()
        {
            
            TestMap = MapLoader.Load("Lobby");
            

            base.LoadContent();
            
        }
    }
}
