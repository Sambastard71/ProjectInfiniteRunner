using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{ 
    public interface IMonotonicClock
    {
        float GetNow();
        float DeltaTime();
        
    }
    
}
