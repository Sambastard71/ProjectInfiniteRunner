using System;
namespace ServerProjectInfiniteRunner
{
    public class Avatar : GameObject
    {
        private Client owner;

        public bool IsOwnedBy(Client client)
        {
            return owner == client;
        }

        public Avatar(uint objectType, Client owner):base(objectType)
        {
        }

        public override void Tick()
        {
            Packet packet = new Packet(Server.COMMAND_UPDATE, Id, RoomId, X, Y, Z);
            // to implement: Server.SendToAllClients(packet);
        }
    }
}