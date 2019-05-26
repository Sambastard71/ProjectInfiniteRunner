using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    class Obstacle : GameObject, IUpdatable
    {
        Collider2D collider;
        bool isCollisionAffected;

        public Obstacle(uint objectType, Vector2 pos, Vector2 vel, Room ownerRoom) : base(objectType, ownerRoom)
        {
            Position.X = pos.X;
            Position.Y = pos.Y;

            Velocity.X = vel.X;
            Velocity.Y = vel.Y;

            collider = new Collider2D(this);

            collider.CollisionType = (uint)UpdateManager.ColliderType.Obstacle;
            collider.AddCollision((uint)UpdateManager.ColliderType.Player);

            UpdateManager.AddItem(this);
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

        public bool GetIsActive()
        {
            return IsActive;
        }

        public bool GetIsCollisionAffected()
        {
            return isCollisionAffected;
        }

        public uint GetRoomId()
        {
            return ownerRoom.ID;
        }

        public void SetIsActive(bool boolean)
        {
            IsActive = boolean;
        }

        public void SetIsCollisionAffected(bool boolean)
        {
            isCollisionAffected = boolean;
        }

        public void Destroy()
        {
            UpdateManager.RemoveItem(this);
        }

        public override void Update()
        {
            base.Update();
            Console.WriteLine("Item id: {0} \n Pos.x: {1} \n Pos.Y: {2}", Id, Position.X, Position.Y);
            Packet packet = new Packet(Server.COMMAND_UPDATE, Id, ownerRoom.ID, Position.X, Position.Y);
            ownerRoom.SendToAllClients(packet);
        }
    }
}
