using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ServerProjectInfiniteRunner
{
    class GameClock : IMonotonicClock
    {
        float beforeTick;
        float currentClock;
        Stopwatch clock;

        public GameClock()
        {
            beforeTick = 0;
            currentClock = 0;
            clock = Stopwatch.StartNew();
        }

        public float GetNow()
        {
            
            return currentClock = clock.ElapsedTicks / Stopwatch.Frequency;

        }

        public float GetDeltaTime()
        {
            long now = clock.ElapsedMilliseconds;
            float dT = (now - beforeTick); // / 1000
            beforeTick = now;
            return dT / 1000;
        }
    }
}
