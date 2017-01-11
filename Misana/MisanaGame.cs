using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Components;
using Misana.Core.Map;

namespace Misana
{
    internal class MisanaGame : Game
    {
        public Map TestMap { get; private set; }

        public ScreenComponent ScreenManager { get; private set; }

        public MisanaGame()
        {
            ScreenManager = new ScreenComponent(this);
            ScreenManager.UpdateOrder = 1;
            ScreenManager.DrawOrder = 1;
            Components.Add(ScreenManager);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            TestMap = MapLoader.Load("Lobby");
        }
    }
}
