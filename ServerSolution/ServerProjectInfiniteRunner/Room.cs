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

        public Vector2[] SpawnersPos;
        

        float countDownStart;

        
        float countDownSpawn;
        public float CountDownSpawn
        {
            get
            {
                return countDownSpawn;
            }
            set
            {
                countDownSpawn = value;
            }
        }
        


        public Room(uint id, Server server)
        {
            serverOwner = server;
            this.id = id;
            players = new Client[2];
            SpawnersPos = new Vector2[2];
            numOfPlayer = 0;
            countDownStart = 4.0f;
            countDownSpawn = 5;
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
                if (players[0] != null && players[1] != null)
                {
                    //(comando, id room, countdown)
                    if (players[0].IsReady && players[1].IsReady)
                    {
                        int cDBeforeSub = (int)countDownStart;
                        countDownStart -= serverOwner.CurrentClock.GetDeltaTime();

                        if (cDBeforeSub != (int)countDownStart)
                        {
                            Packet countDownPacket = new Packet(Server.COMMAND_COUNTDOWN, ID, (int)countDownStart);
                            SendToAllClients(countDownPacket);
                            Console.WriteLine(countDownStart);
                        }
                        
                        if (countDownStart <= 0)
                        {
                            IsStartLoading = false;
                        }
                    }
                }
            }
            else
            {
                SpawnManager.Spawn(this);
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
