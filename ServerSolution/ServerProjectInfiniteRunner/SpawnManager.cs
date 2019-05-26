using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    static class SpawnManager
    {
        public static void Spawn(Room room)
        {
            
            room.CountDownSpawn -= room.Server.CurrentClock.GetDeltaTime();

            if (room.CountDownSpawn <= 0)
            {
                Random rand = new Random();

                uint obstacleType = (uint)rand.Next(1, 3);
                int laneWhereSpawn = rand.Next(0, 1);

                Vector2 pos = room.SpawnersPos[laneWhereSpawn];
                Vector2 vel = new Vector2(-10, 0);

                Obstacle obstacle = new Obstacle(obstacleType, pos, vel, room);

                Packet SpawnObject = new Packet(Server.COMMAND_SPAWN, room.ID, obstacle.Id, obstacleType, laneWhereSpawn+1);
                room.SendToAllClients(SpawnObject);
            }
            
        }
    }
}
