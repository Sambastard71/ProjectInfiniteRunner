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
        
        Client[] players;
        
        public Room(uint id, Client player1)
        {
            this.id = id;
            players = new Client[MAX_NUM_OF_PLAYER];
            players[0] = player1;
            numOfPlayer = 1;    
        }

        public Room(uint id)
        {
            this.id = id;
            players = new Client[MAX_NUM_OF_PLAYER];
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
            return true;
        }
    }
}
