using System;

namespace Misana.Core
{
    public struct GameTime
    {
        public TimeSpan ElapsedTime;
        public TimeSpan TotalTime;

        public GameTime(TimeSpan elapsedGameTime, TimeSpan totalGameTime)
        {
            ElapsedTime = elapsedGameTime;
            TotalTime = totalGameTime;
        }
    }
}