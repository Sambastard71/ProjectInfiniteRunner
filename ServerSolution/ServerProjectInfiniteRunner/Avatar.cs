using System;
namespace ServerProjectInfiniteRunner
{
    public class Avatar : GameObject,IUpdatable, ISpawnable
    {
        Collider2D collider;
        bool isCollisionAffected;

        public Avatar(uint objectType):base(objectType)
        {
            UpdateManager.AddItem(this);
            SpawnManager.AddItem(this);

            collider.CollisionType = (uint)UpdateManager.ColliderType.Player;
            collider.AddCollision((uint)UpdateManager.ColliderType.Obstacle);
        }

        public override void Update()
        {
            base.Update();
            Packet packet = new Packet(Server.COMMAND_UPDATE, Id, ownerRoom.ID, XPos, YPos, ZPos);
            //Send to all clients in room
        }

        public void Spawn()
        {
            Packet packet = new Packet(Server.COMMAND_SPAWN, Id, ObjectType ,ownerRoom.ID, XPos, YPos, ZPos);

            //Send to all clients in room
        }

        public override void OnCollide(Collision collisionInfo)
        {
            base.OnCollide(collisionInfo);
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
            return (collider.CollisionMask & collider.CollisionMask) != 0;
        }

        public Collider2D GetCollider()
        {
            return collider;
        }

        public GameObject GetGameObject()
        {
            return this;
        }
    }
}