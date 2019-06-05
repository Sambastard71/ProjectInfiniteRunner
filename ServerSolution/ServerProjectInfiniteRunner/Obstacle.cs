using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    public class Obstacle : GameObject, IUpdatable
    {
        Collider2D collider;
        const float DESPAWN_OFFSET = 225;

        public Obstacle(uint objectType, Vector3 pos, Vector3 vel, Room ownerRoom) : base(objectType, ownerRoom)
        {

            SetPosition(pos.X, pos.Y, pos.Z);
            SetVelocity(vel.X, vel.Y, vel.Z);

            Width = 25;
            Height = 45;


            collider = new Collider2D(this);

            collider.CollisionType = (uint)UpdateManager.ColliderType.Obstacle;
            collider.AddCollision((uint)UpdateManager.ColliderType.Player);

            UpdateManager.AddItem(this);
        }

        //public bool CheckCollisionWith(Collider2D collider)
        //{
        //    return (this.collider.CollisionType & collider.CollisionMask) == this.collider.CollisionType;
        //}

        public bool CheckCollisionWith(Collider2D collider)
        {
            return (this.collider.CollisionMask & collider.CollisionType) != 0;
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
            Packet Destroy = new Packet(Server.COMMAND_DESTROY, ownerRoom.ID, Id);
            ownerRoom.SendToAllClients(Destroy);
        }

        public override void Update()
        {
            base.Update();
            if ((Position.X + DESPAWN_OFFSET) < ownerRoom.Players[0].Avatar.Position.X)
            {
                Destroy();
            }
            
        }

        public override void OnCollide(Collision collisionInfo)
        {
            if(collisionInfo.Collider.Id==1)
                Console.WriteLine("Collided player1");
            else if(collisionInfo.Collider.Id == 2)
                Console.WriteLine("Collided player2");

        }

        public override void SendUpdate()
        {
            counterToUpdate -= 0.02f;
            if (counterToUpdate <= 0)
            {
                Packet packet = new Packet(Server.COMMAND_UPDATE, Id, ownerRoom.ID, Position.X, Position.Y, Position.Z);
                ownerRoom.SendToAllClients(packet);

                counterToUpdate = Server.TIME_TO_SEND_UPDATE;
            }
        }
    }
}
