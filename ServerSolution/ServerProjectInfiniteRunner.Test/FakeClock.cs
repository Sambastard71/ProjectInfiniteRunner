using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner.Test
{
    class FakeClock //: IMonotonicClock
    {
        float timeStamp;

        public FakeClock(float timeStamp = 0)
        {
            this.timeStamp = timeStamp;
        }

        public float GetNow()
        {
            return timeStamp;
        }
        
        public void IncreaseTimeStamp(float delta)
        {
            if (delta <= 0)
            {
                throw new Exception("Invalid delta value");
            }
            timeStamp += delta;
        }
    }
}
