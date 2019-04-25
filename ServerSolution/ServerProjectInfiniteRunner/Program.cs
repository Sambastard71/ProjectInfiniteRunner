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
            List<Client> clients = new List<Client>();
            TransportIPv4 transport = new TransportIPv4();
            transport.Bind("127.0.0.1", 9999);

            Server server = new Server(transport);
            server.Start();

        }
    }
}
