﻿using System;
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

        public const byte COMMAND_JOIN = 1;
        public const byte COMMAND_UPDATE = 2;
        public const byte COMMAND_SPAWN = 3;
        public const byte COMMAND_ERROR = 4;
        public const byte COMMAND_WELCOME = 5;
        public const byte COMMAND_SETUP = 6;
        public const byte COMMAND_INTANGIBLE = 7;
        public const byte COMMAND_P_CONNECTED = 8;
        public const byte COMMAND_SETUP_OP = 9;
        public const byte COMMAND_COUNTDOWN = 10;



        ITransport transport;
        IMonotonicClock clock;
        command[] commands;

        //List of clients in connected
        List<Client> clients;

        public int numOfClients
        {
            get { return clients.Count; }
        }

        public IMonotonicClock CurrentClock
        {
            get { return clock; }
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
        public Server(ITransport transport, IMonotonicClock gameClock)
        {
            this.transport = transport;

            clock = gameClock;

            commands = new command[COMMAND_P_CONNECTED + 1];

            commands[COMMAND_JOIN] = Join;
            commands[COMMAND_SETUP] = SetUp;
            commands[COMMAND_INTANGIBLE] = Intangible;

            clients = new List<Client>();

            rooms = new List<Room>();
            rooms.Add(new Room((uint)numOfRooms, this));
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

        public void SingleStep()
        {
            //if Game in a room is starting do this
            foreach (Room room in Rooms)
            {
                room.Process();
            }

            CurrentClock.GetDeltaTime();
            EndPoint sender = transport.CreateEndPoint();
            byte[] data = transport.Recv(256, ref sender);

            if (data == null)
            {
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
        //Update
        //Spawn

        //Join Method For Quick Game && Classic Game
        private void Join(byte[] packet, EndPoint endPoint)
        {
            Client c = new Client(endPoint, this);

            //Check if the client is already joined
            if (packet.Length != 1 || clients.Exists(client => client.EndPoint.Equals(c.EndPoint)))
            {
                foreach (Client client in clients)
                {
                    if (client.EndPoint.Equals(c.EndPoint))
                    {
                        client.malus -= 10;
                    }
                }

                c.Destroy();
                //clients.Remove(c);

                return;
            }

            //Adding the client to the clients list
            clients.Add(c);

            //Check for empty room
            uint roomId = (uint)numOfRooms - 1;
            uint IdinRoom;
            
            if (rooms[numOfRooms - 1].NumOfPlayer <= 1)
            {
                Room room = rooms[numOfRooms - 1];

                room.AddPlayer(c);

                IdinRoom = (uint)room.NumOfPlayer;

                c.SetRoom(room);

                Packet welcome = new Packet(COMMAND_WELCOME, IdinRoom, roomId);
                c.Enqueue(welcome);

                if (room.NumOfPlayer ==  2)
                {
                    Packet player1Connect = new Packet(COMMAND_P_CONNECTED, 1, roomId);
                    Packet player2Connect = new Packet(COMMAND_P_CONNECTED, 2, roomId);

                    room.Players[0].Enqueue(player2Connect);
                    room.Players[1].Enqueue(player1Connect);
                }
            }
            else if (rooms[numOfRooms - 1].NumOfPlayer == 2)
            {
                Room room = new Room(roomId, this);

                Rooms.Add(room);
                room.AddPlayer(c);
                IdinRoom = (uint)room.NumOfPlayer;

                c.SetRoom(room);

                Packet welcome = new Packet(COMMAND_WELCOME, IdinRoom, roomId);

                c.Enqueue(welcome);

            }
            
            Console.WriteLine("client {0} joined with avatar {1}", c.ID, c.Avatar.Id);
        }


        // (comando,idpersonaggioNellaStanza,idRoom,xpos,ypos,zpos,width,height collider)
        private void SetUp(byte[] packet, EndPoint endPoint)
        {
            Client c; // = new Client(endPoint, this);

            if (packet.Length != 29 )
            {
                foreach (Client client in clients)
                {
                    if (client.EndPoint.Equals(endPoint))
                    {
                        client.malus -= 10;
                    }
                }

                return;
            }

            if (!clients.Exists(client => client.EndPoint.Equals(endPoint)))
            {
                return;
            }

            uint idPersonaggio = BitConverter.ToUInt32(packet, 1);
            uint idRoom = BitConverter.ToUInt32(packet, 5);
            float xPos = BitConverter.ToSingle(packet, 9);
            float yPos = BitConverter.ToSingle(packet, 12);
            float zPos = BitConverter.ToSingle(packet, 15);
            float width = BitConverter.ToSingle(packet, 18);
            float height = BitConverter.ToSingle(packet, 21);


            Room room = Rooms[(int)idRoom];
            c = room.Players[(int)idPersonaggio - 1];

            c.Avatar.XPos = xPos;
            c.Avatar.YPos = yPos;
            c.Avatar.ZPos = zPos;
            c.Avatar.Width = width;
            c.Avatar.Height = height;
            c.IsReady = true;

            Packet setUpOp = new Packet(COMMAND_SETUP_OP, idPersonaggio, idRoom, xPos, yPos, zPos);

            if (idPersonaggio == 1)
            {
                rooms[(int)idRoom].Players[1].Enqueue(setUpOp);
            }
            else if(idPersonaggio == 2)
            {
                rooms[(int)idRoom].Players[0].Enqueue(setUpOp);
            }
        }

        // (comando,idpersonaggioNellaStanza,idRoom)
        private void Intangible(byte[] packet, EndPoint endPoint)
        {
            Client c = new Client(endPoint, this);

            if (packet.Length != 9 || !clients.Exists(client => client.EndPoint.Equals(c.EndPoint)))
            {
                foreach (Client client in clients)
                {
                    if (client.EndPoint.Equals(c.EndPoint))
                    {
                        client.malus -= 10;
                    }
                }

                return;
            }

            uint idPersonaggio = BitConverter.ToUInt32(packet, 1);
            uint idRoom = BitConverter.ToUInt32(packet, 5);

            Room room = Rooms[(int)idRoom];
            c = room.Players[(int)idPersonaggio];

            c.Avatar.SetIsCollisionAffected(false);

        }
    }
}
