using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    public static class SpawnManager
    {

        public static Obstacle Spawn(Room room, int lane)
        {

            Random rand = new Random();

            uint obstacleType = (uint)rand.Next(2, 5);

            int laneWhereSpawn = lane;


            Vector3 pos = room.SpawnersPos[laneWhereSpawn - 1];
            Vector3 vel = new Vector3(-1f, 0, 0);

            Obstacle obstacle = new Obstacle(obstacleType, pos, vel, room);

            Console.WriteLine("Spawn Obstacle {0}", obstacle.Id);

            Packet SpawnObject = new Packet(Server.COMMAND_SPAWN, room.ID, obstacle.Id, obstacleType, laneWhereSpawn);
            room.SendToAllClients(SpawnObject);

            return obstacle;
        }
    }
}
