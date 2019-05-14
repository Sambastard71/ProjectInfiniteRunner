using System;
namespace ServerProjectInfiniteRunner
{
    public class Avatar : GameObject,IUpdatable, ISpawnable
    {

        public Avatar(uint objectType):base(objectType)
        {
            UpdateManager.AddItem(this);
            SpawnManager.AddItem(this);
        }

        public override void Update()
        {
            base.Update();
            Packet packet = new Packet(Server.COMMAND_UPDATE, Id, ownerRoom.ID, XPos, YPos, ZPos);
            //Send to all clients in room
        }

        public void Spawn()
        {
            Packet packet = new Packet(Server.COMMAND_SPAWN, Id, ownerRoom.ID, XPos, YPos, ZPos);

            //Send to all clients in room
        }
    }
}