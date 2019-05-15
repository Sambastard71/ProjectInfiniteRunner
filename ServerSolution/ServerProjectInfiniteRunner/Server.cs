using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;


namespace ServerProjectInfiniteRunner
{
    delegate void command(byte[] packet, EndPoint sender);

    public class Server
    {

        const int MAX_ROOM = 32;

        //Command List To Update

        public const int COMMAND_JOIN = 1;
        public const int COMMAND_UPDATE = 2;
        public const int COMMAND_SPAWN = 3;
        public const int COMMAND_SETUP = 6;
        public const int COMMAND_INTANGIBLE = 7;


        public const int COMMAND_ERROR = 4;
        public const int COMMAND_WELCOME = 5;

        ITransport transport;
        IMonotonicClock clock;
        command[] commands;

        //List of clients in connected
        List<Client> clients;

        
        public IMonotonicClock CurrentClock
        {
            get { return clock; }
        }

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

            clock = new GameClock();

            commands = new command[COMMAND_SPAWN];
            
            commands[COMMAND_JOIN] = Join;
            commands[COMMAND_SETUP] = SetUp;
            commands[COMMAND_INTANGIBLE] = Intangible;




            clients = new List<Client>();
            rooms = new List<Room>();
            
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine(clock.GetDeltaTime());
                SingleStep();
            }

            
        }

        //Dispatcher that recive packet and call every right command
        //TODO
        //Add Malus To Client who try to send empty packet
        
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

            //if Game in a room is starting do this
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
            uint roomId = (uint)numOfRooms;
            uint IdinRoom = 0;

            if (packet[0] == COMMAND_JOIN)
            {
                if (numOfRooms == 0)
                {
                    Room room = new Room(roomId, this);
                    Rooms.Add(room);
                    room.AddPlayer(c);
                    IdinRoom = (uint)room.NumOfPlayer;

                    Packet welcome = new Packet(COMMAND_WELCOME, roomId, IdinRoom);
                    c.Enqueue(welcome);
                }

                if (numOfRooms >= 1)
                {
                    if (rooms[numOfRooms - 1].NumOfPlayer == 1)
                    {
                        rooms[numOfRooms - 1].AddPlayer(c);
                        IdinRoom = (uint)rooms[numOfRooms - 1].NumOfPlayer;

                        Packet welcome = new Packet(COMMAND_WELCOME, IdinRoom, roomId);
                        c.Enqueue(welcome);
                    }
                    else if (rooms[numOfRooms - 1].NumOfPlayer == 2)
                    {
                        Room room = new Room(roomId, this);
                        Rooms.Add(room);
                        room.AddPlayer(c);
                        IdinRoom = (uint)room.NumOfPlayer;

                        Packet welcome = new Packet(COMMAND_WELCOME, IdinRoom, roomId);
                        c.Enqueue(welcome);
                    }
                }


                //foreach (Room room in Rooms)
                //{
                //    if (room.NumOfPlayer <= 1)
                //    {
                //        playerJoined = room.AddPlayer(c);
                //        roomId = room.ID;
                //        playerJoined = true;
                //    }
                //}

            }

            //if there isn't an empty room create it and add the client to it
            //if (!playerJoined || packet[0] == COMMAND_JOIN)
            //{
            //    Room r = null;
            //    if (rooms.Count < MAX_ROOM)
            //    {
            //        r = new Room((uint)rooms.Count, c,this);
            //        idInRoom = 1;
            //    }
            //    else
            //    {
            //        Packet error = new Packet(COMMAND_ERROR, "Max number of room reached try to reconnect");
            //        transport.Send(error.GetData(), endPoint);
            //        return;
            //    }

            //    if (r != null)
            //    {
            //        rooms.Add(r);
            //        roomId = r.ID;
            //    }
            //}


            ////TODO make welcomePacket and send it


            //transport.Send(welcome.GetData(), endPoint);
        }


        // (comando,idpersonaggioNellaStanza,idRoom,xpos,ypos,zpos,width,height collider)
        private void SetUp(byte[] packet, EndPoint endPoint)
        {
            Client c = new Client(endPoint, this);

            //check lenght of packet
            if (packet.Length == 24)
            {
                uint idPersonaggio = BitConverter.ToUInt32(packet, 1);
                uint idRoom = BitConverter.ToUInt32(packet, 5);
                float xPos = BitConverter.ToSingle(packet, 9);
                float yPos = BitConverter.ToSingle(packet, 12);
                float zPos = BitConverter.ToSingle(packet, 15);
                float width = BitConverter.ToSingle(packet, 18);
                float height = BitConverter.ToSingle(packet, 21);

                //check if client exist
                if (clients.Exists(client => client.EndPoint.Equals(c.EndPoint)))
                {
                    Room room = Rooms[(int)idRoom];
                    Client client = room.Players[(int)idPersonaggio];

                    client.Avatar.XPos = xPos;
                    client.Avatar.YPos = yPos;
                    client.Avatar.ZPos = zPos;
                    client.Avatar.Width = width;
                    client.Avatar.Height = height;

                }
                else
                {
                    //TODO add Malus to the client

                }
            }
            else
            {
                //TODO add Malus to the client
            }
        }

        // (comando,idpersonaggioNellaStanza,idRoom)
        private void Intangible(byte[] packet, EndPoint endPoint)
        {
            Client c = new Client(endPoint, this);

            //check lenght of packet
            if (packet.Length == 9)
            {
                uint idPersonaggio = BitConverter.ToUInt32(packet, 1);
                uint idRoom = BitConverter.ToUInt32(packet, 5);
                

                //check if client exist
                if (clients.Exists(client => client.EndPoint.Equals(c.EndPoint)))
                {
                    Room room = Rooms[(int)idRoom];
                    Client client = room.Players[(int)idPersonaggio];

                    client.Avatar.SetIsCollisionAffected(false);
                }
                else
                {
                    //TODO add Malus to the client

                }
            }
            else
            {
                //TODO add Malus to the client
            }
        }


    }
}
