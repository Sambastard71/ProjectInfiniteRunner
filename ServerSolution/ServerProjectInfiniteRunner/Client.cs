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

        private Server connectedServer;

        public Client(EndPoint endPoint, Server server)
        {
            this.endPoint = endPoint;
            this.connectedServer = server;
            sendQueue = new Queue<Packet>();
            waitingForAck = new Dictionary<uint, Packet>();
        }

        public void Process()
        {
            int packetToSend = sendQueue.Count;
            for (int i = 0; i < packetToSend; i++)
            {
                Packet packet = sendQueue.Dequeue();
                if (connectedServer.Send(packet.GetData(), endPoint))
                {
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
    }
}
