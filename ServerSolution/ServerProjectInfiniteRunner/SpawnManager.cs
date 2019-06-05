using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    static class SpawnManager
    {

        public static void Spawn(Room room, int lane)
        {

                Random rand = new Random();

                uint obstacleType = (uint)rand.Next(2, 5);

                int laneWhereSpawn = lane;
                //rand.Next(1, 3);

                Vector3 pos = room.SpawnersPos[laneWhereSpawn-1];
                Vector3 vel = new Vector3(-2f, 0,0);

                Obstacle obstacle = new Obstacle(obstacleType, pos, vel, room);
                

                Packet SpawnObject = new Packet(Server.COMMAND_SPAWN, room.ID, obstacle.Id, obstacleType, laneWhereSpawn);
                room.SendToAllClients(SpawnObject);
                

               
            
            
        }
    }
}
