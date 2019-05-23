using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            
            TransportIPv4 transport = new TransportIPv4();
            GameClock gameClock = new GameClock();

            transport.Bind("192.168.1.191", 9998);

            Server server = new Server(transport, gameClock);
            server.Start();

        }
    }
}
