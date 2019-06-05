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

            Console.WriteLine("Inserisci L'ip");
            string ip = Console.ReadLine();
            Console.WriteLine("Inserisci la porta");
            string porta = Console.ReadLine();

            transport.Bind(ip, int.Parse(porta));

            Server server = new Server(transport, gameClock);
            server.Start();

        }
    }
}
