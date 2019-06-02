using System;
namespace ServerProjectInfiniteRunner
{
    public class Avatar : GameObject,IUpdatable
    {
        Collider2D collider;
        
        
        public Avatar(uint objectType,Room room):base(objectType,room)
        {
            ownerRoom = room;

            Width = 40;
            Height = 15;


            collider = new Collider2D(this);

            collider.CollisionType = (uint)UpdateManager.ColliderType.Player;
            collider.AddCollision((uint)UpdateManager.ColliderType.Obstacle);

            UpdateManager.AddItem(this);
        }

        public override void Update()
        {
            //base.Update();

            //counterToUpdate -= ownerRoom.Server.CurrentClock.GetDeltaTime();
            //if (counterToUpdate <= 0)
            //{
            //Packet packet = new Packet(Server.COMMAND_UPDATE, Id, ownerRoom.ID, Position.X, Position.Y);
            //Send to all clients in room
            //}
        }

        public override void OnCollide(Collision collisionInfo)
        {
            UpdateManager.AddItemToRemoveList(this);

            Packet packet = new Packet(Server.COMMAND_COLLIDE, Id, ownerRoom.ID);
            ownerRoom.SendToAllClients(packet);
        }

        public bool GetIsCollisionAffected()
        {
            return isCollisionAffected;
        }

        public void SetIsCollisionAffected(bool boolean)
        {
            isCollisionAffected = boolean;
        }

        public bool GetIsActive()
        {
            return IsActive;
        }

        public void SetIsActive(bool boolean)
        {
            IsActive = boolean;
        }

        public uint GetRoomId()
        {
            return ownerRoom.ID;
        }

        public bool CheckCollisionWith(Collider2D collider)
        {
            return (this.collider.CollisionType & collider.CollisionMask) == this.collider.CollisionType;
        }

        public Collider2D GetCollider()
        {
            return collider;
        }

        public GameObject GetGameObject()
        {
            return this;
        }

        public void Destroy()
        {
            UpdateManager.RemoveItem(this);
        }

        
    }
}