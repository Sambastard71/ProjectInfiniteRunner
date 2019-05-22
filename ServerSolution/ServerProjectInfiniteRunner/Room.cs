using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    public class Room
    {
        const int MAX_NUM_OF_PLAYER = 2;

        private bool IsStartLoading = true;

        private Server serverOwner;
        public Server Server
        {
            get
            {
                return serverOwner;
            }
        }

        uint id;
        public uint ID
        {
            get
            {
                return id;
            }
        }

        int numOfPlayer;
        public int NumOfPlayer
        {
            get
            {
                return numOfPlayer;
            }
        }
        
        private Client[] players;

        public  Client[] Players
        {
            get { return players; }
        }
        


        public Room(uint id, Server server)
        {
            serverOwner = server;
            this.id = id;
            players = new Client[2];
            numOfPlayer = 0;
       
        }

        public bool AddPlayer(Client player)
        {
            if (NumOfPlayer >= MAX_NUM_OF_PLAYER)
            {
                return false;
            }

            players[NumOfPlayer] = player;
            numOfPlayer++;
            player.SetRoom(this);
            return true;
        }

        public void Process()
        {
            foreach (Client client in players)
            {
                if(client!=null)
                client.Process();
            }

            if (IsStartLoading)
            {
                foreach(Client client in players)
                {
                    
                    
                }

                SpawnManager.Spawn();
                SpawnManager.RemoveAll();
                IsStartLoading = false;

            }

            CheckMalus();
            //spawn of obstacles

            UpdateManager.CheckCollisions();

            UpdateManager.Update();
        }

        public void SendToAllClients(Packet packet)
        {
            foreach (Client client in players)
            {
                client.Enqueue(packet);
            }
        }

        private void CheckMalus()
        {
            foreach (Client client in players)
            {
                if (client!=null && client.malus <= -50)
                {
                    int Id = client.ID;
                    players[Id] = null;
                    Console.WriteLine("Client kicked out");
                    numOfPlayer--;
                }
            }
        }
    }
}
