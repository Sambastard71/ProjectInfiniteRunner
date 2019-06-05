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
        float nowTick;
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
            currentClock = (float)(clock.ElapsedTicks) / (float)(Stopwatch.Frequency);
            beforeTick = nowTick;
            nowTick = currentClock;
            return currentClock;
        }

        public float DeltaTime()
        {
            float deltaTime = (nowTick - beforeTick);
            //Console.WriteLine(deltaTime);
            return deltaTime;
        }
    }
}
