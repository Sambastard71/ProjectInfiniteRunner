using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    public static class SpawnManager
    {
        const int RANDOM_COUNTER = 5;

        public static Obstacle Spawn(Room room, int lane)
        {

            Random rand = new Random();
            int counter = RANDOM_COUNTER;
            float seed = 0;

            while (counter > 0)
            {
                seed += Server.CurrentClock.DeltaTime() + rand.Next(1, 100);
                counter--;
            }
            Random randType = new Random((int)seed); 
            uint obstacleType = (uint)randType.Next(2, 4);

            int laneWhereSpawn = lane;
            int subLane = rand.Next(1, 3);
            
            Vector3 pos = room.SpawnersPos[laneWhereSpawn - 1];
            Vector3 vel = new Vector3(-50f, 0, 0);
            
            float Z = pos.Z;
            if (obstacleType != 2)
            {
                if (subLane == 1)
                {
                    Z = pos.Z - 25;
                }
                else if(subLane == 2)
                {
                    Z = pos.Z + 25;
                }

            }
            
            pos  = new Vector3(pos.X, pos.Y, Z);

            Obstacle obstacle = new Obstacle(obstacleType, pos, vel, room);
            Console.WriteLine("Spawn Obstacle {0}", obstacle.Id);
            Console.WriteLine("obstacleType: " + obstacleType + " obstacle lane: " + laneWhereSpawn);
            Packet SpawnObject = new Packet(Server.COMMAND_SPAWN, room.ID, obstacle.Id, obstacleType, pos.X, pos.Y, pos.Z);
            room.SendToAllClients(SpawnObject);

            return obstacle;
        }
    }
}
