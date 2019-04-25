using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ServerProjectInfiniteRunner
{
    delegate void command(byte[] packet, EndPoint sender);

    public class Server
    {
        //Command List To Update
        const int COMMAND_JOIN = 0;
        const int COMMAND_UPDATE = 1;
        const int COMMAND_SPAWN = 2;


        ITransport transport;
        command[] commands;

        //List of clients in connected
        List<Client> clients;

        public List<Client> Clients
        {
            get
            {
                return clients;
            }
        }

        public int numOfClient
        {
            get
            {
                return clients.Count;
            }
        }

        //List of actual active rooms
        List<Room> rooms;
        public List<Room> Rooms
        {
            get
            {
                return rooms;
            }
        }

        //constructor that init the server parameters
        public Server(ITransport transport)
        {
            this.transport = transport;

            commands = new command[COMMAND_SPAWN + 1];
            commands[COMMAND_JOIN] = Join;
            commands[COMMAND_UPDATE] = Update;
            commands[COMMAND_SPAWN] = Spawn;

            clients = new List<Client>();
            rooms = new List<Room>();
            rooms.Add(new Room((uint)rooms.Count));
        }
        
        public void Start()
        {
            while (true)
            {
                SingleStep();
            }
        }

        //Dispatcher that recive packet and call every right command
        //TODO
        //Add Malus To Client who try to send empty packet
        //Add Timeing of server
        public void SingleStep()
        {

            EndPoint sender = transport.CreateEndPoint();
            byte[] data = transport.Recv(256, ref sender);

            if (data == null)
            {
                //TODO Add Malus to Client
                return;
            }

            commands[data[0]](data, sender);
            
        }

        //Method that send packet to the client
        public bool Send(byte[] packet, EndPoint endPoint)
        {
            return transport.Send(packet, endPoint);
        }


        //TODO
        //Make The commands
        //Welcome
        //Update
        //Spawn
        private void Join(byte[] packet, EndPoint endPoint)
        {
            //Check the lenght of the join packet
            if (packet.Length > 1)
            {
                //TODO Add Malus To Client
                return;
            }

            Client c = new Client(endPoint, this);

            //Check if the client is already joined
            if (Clients.Exists(x => x.EndPoint.Equals(c.EndPoint)))
            {
                //TODO Add Malus To Client
                return;
            }

            //Adding the client to the clients list
            clients.Add(c);

            //Check for empty room
            bool playerJoined = false;
            foreach (Room room in Rooms)
            {
                if (room.NumOfPlayer <= 1)
                {
                    playerJoined = room.AddPlayer(c);
                }
            }

            //if there isn't an empty room create it and add the client to it
            if (!playerJoined)
            {
                Room r = new Room((uint)rooms.Count, c);
                rooms.Add(r);
            }

            //TODO make welcomePacket and send it
        }

        private void Welcome(EndPoint endPoint)
        {

        }

        public void Update(byte[] packet, EndPoint endPoint)
        {
        }

        public void Spawn(byte[] packet, EndPoint endPoint)
        {
        }

    }
}
