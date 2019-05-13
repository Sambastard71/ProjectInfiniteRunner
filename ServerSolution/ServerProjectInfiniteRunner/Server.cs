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

        const int MAX_ROOM = 32;

        //Command List To Update
        public const int COMMAND_FAST_JOIN = 0;
        public const int COMMAND_JOIN = 1;
        public const int COMMAND_UPDATE = 2;
        public const int COMMAND_SPAWN = 3;

        public const int COMMAND_ERROR = 4;
        public const int COMMAND_WELCOME = 5;

        ITransport transport;
        command[] commands;

        //List of clients in connected
        List<Client> clients;

        public int numOfClients
        {
            get
            {
                return clients.Count;
            }
        }

        public List<Client> Clients
        {
            get
            {
                return clients;
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

        public int numOfRooms
        {
            get
            {
                return Rooms.Count;
            }
        }

        //constructor that init the server parameters
        public Server(ITransport transport)
        {
            this.transport = transport;

            commands = new command[COMMAND_SPAWN + 1];
            commands[COMMAND_FAST_JOIN] = Join;
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

            foreach (Room room in Rooms)
            {
                room.Process();
            }
        }

        //Method that send packet to the client
        public bool Send(byte[] packet, EndPoint endPoint)
        {
            return transport.Send(packet, endPoint);
        }


        //TODO
        //Make The commands
        //Update
        //Spawn

        //Join Method For Quick Game && Classic Game
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
            if (clients.Exists(client => client.EndPoint.Equals(c.EndPoint)))
            {
                //TODO add Malus to the client
                return;
            }

            //Adding the client to the clients list
            clients.Add(c);

            //Check for empty room
            bool playerJoined = false;
            uint roomId = uint.MaxValue;
            int idInRoom = 2;

            if (packet[0] == COMMAND_FAST_JOIN)
            {
                foreach (Room room in Rooms)
                {
                    if (room.NumOfPlayer <= 1)
                    {
                        playerJoined = room.AddPlayer(c);
                        roomId = room.ID;
                        playerJoined = true;
                    }
                }

            }

            //if there isn't an empty room create it and add the client to it
            if (!playerJoined || packet[0] == COMMAND_JOIN)
            {
                Room r = null;
                if (rooms.Count < MAX_ROOM)
                {
                    r = new Room((uint)rooms.Count, c);
                    idInRoom = 1;
                }
                else
                {
                    Packet error = new Packet(COMMAND_ERROR, "Max number of room reached try to reconnect");
                    transport.Send(error.GetData(), endPoint);
                    return;
                }

                if (r != null)
                {
                    rooms.Add(r);
                    roomId = r.ID;
                }
            }


            //TODO make welcomePacket and send it
            Packet welcome = new Packet(COMMAND_WELCOME, roomId, idInRoom);

            transport.Send(welcome.GetData(), endPoint);
        }
        
        public void Update(byte[] packet, EndPoint endPoint)
        {
        }

        public void Spawn(byte[] packet, EndPoint endPoint)
        {
        }

    }
}
