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
        const float RESET_SPAWN_TIMER = 3.0f;
        const float TIMER = 0.01f;

        private bool IsStartLoading = true;

        private bool spawn = false;

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

        public Vector3[] SpawnersPos;
        

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

        private float spawnTimer;
        public float SpawnTimer
        {
            get
            {
                return spawnTimer;
            }
            set
            {
                spawnTimer = value * 0.99f;
                if (spawnTimer <= 1)
                {
                    spawnTimer = RESET_SPAWN_TIMER;
                }
            }
        }

        public Room(uint id, Server server)
        {
            serverOwner = server;
            this.id = id;
            players = new Client[2];
            SpawnersPos = new Vector3[2];
            numOfPlayer = 0;
            countDownStart = 4.0f;
            countDownSpawn = RESET_SPAWN_TIMER;
            
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
                    CountDown();
                }
            }
            else if(spawn)
            {
                Spawn();
            }
            
            CheckMalus();

            UpdateManager.Update();

            UpdateManager.CheckCollisions();

        }

        public void SendToAllClients(Packet packet)
        {
            if (players[0] != null && players[1] != null)
            {
                foreach (Client client in players)
                {
                    client.Enqueue(packet);
                }
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

        private void CountDown()
        {
            if (players[0] != null && players[1] != null)
            {
                //(comando, id room, countdown)
                if (players[0].IsReady && players[1].IsReady)
                {
                    int cDBeforeSub = (int)countDownStart;
                    countDownStart -= TIMER;

                    if (cDBeforeSub != (int)countDownStart)
                    {
                        Packet countDownPacket = new Packet(Server.COMMAND_COUNTDOWN, ID, (int)countDownStart);
                        SendToAllClients(countDownPacket);
                        Console.WriteLine(countDownStart);
                    }

                    if (countDownStart <= 0)
                    {
                        IsStartLoading = false;
                        spawn = true;
                    }
                }
            }
        }

        private void Spawn()
        {
            CountDownSpawn -= TIMER;
            if (countDownSpawn <= 0)
            {
                Random rand = new Random();
                int lane = rand.Next(1, 3);

                SpawnManager.Spawn(this, lane);

                countDownSpawn = spawnTimer;
                Console.WriteLine("SpawnTimer: "+spawnTimer);
                SpawnTimer = spawnTimer;
            }
        }
    }
}
