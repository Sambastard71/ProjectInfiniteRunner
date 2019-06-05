using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ServerProjectInfiniteRunner
{
    public class Client
    {
        private Avatar avatar;
        public Avatar Avatar
        {
            get { return avatar; }
        }

        private Room ownerRoom;
        public Room OwnerRoom
        {
            get { return ownerRoom; }
        }

        public float malus = 0;

        private EndPoint endPoint;
        public EndPoint EndPoint
        {
            get
            {
                return endPoint;
            }
        }
        private Queue<Packet> sendQueue;

        private Dictionary<uint, Packet> waitingForAck;

        private bool isReady;
        public bool IsReady
        {
            get
            {
                return isReady;
            }
            set
            {
                isReady = true;
            }
        }

        private Server connectedServer;
        private static int numberOfPlayer;

        private int id;
        public int ID
        {
            get
            {
                return id;
            }
        }

        public Client(EndPoint endPoint, Server server)
        {
            this.endPoint = endPoint;
            this.connectedServer = server;

            sendQueue = new Queue<Packet>();
            waitingForAck = new Dictionary<uint, Packet>();
            id = numberOfPlayer++;
            avatar = new Avatar(1, ownerRoom);
        }

        public void Process()
        {
            int packetToSend = sendQueue.Count;
            for (int i = 0; i < packetToSend; i++)
            {
                Packet packet = sendQueue.Dequeue();
                if (connectedServer.Send(packet.GetData(), endPoint))
                {
                    //Console.WriteLine("Client" + id + " has sent " + packet.GetData()[0] + " Packet");
                    if (packet.NeedsAck)
                    {
                        waitingForAck[packet.ID] = packet;
                    }
                }
                else
                {
                    sendQueue.Enqueue(packet);
                }

            }

            //Controllo Sui pacchetti scaduti
            List<uint> packetToRemove = new List<uint>();
            //foreach (uint key in waitingForAck.Keys)
            //{
            //    Packet packet = waitingForAck[key];
            //    if (packet.IsExpired)
            //    {
            //        packetToRemove.Add(key);
            //        if (packet.Attempts < 3)
            //        {
            //            packet.IncreaseAttempts();
            //            sendQueue.Enqueue(packet);
            //        }
            //    }
            //}

            foreach (uint packetId in packetToRemove)
            {
                waitingForAck.Remove(packetId);
            }

            
        }

        public void CheckAck(uint id)
        {
            if (waitingForAck.ContainsKey(id))
            {
                waitingForAck.Remove(id);
            }
        }

        public void Enqueue(Packet packet)
        {
            sendQueue.Enqueue(packet);
        }

        public void SetRoom(Room room)
        {
            ownerRoom = room;
            avatar.ownerRoom = room;
        }

        public void Destroy()
        {
            avatar.Destroy();
        }

        public static void ResetNumberOfPlayer()
        {
            numberOfPlayer = 0;
        }
    }
}
