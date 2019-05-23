using System;
namespace ServerProjectInfiniteRunner
{
    public abstract class GameObject
    {
        public float XPos;
        public float YPos;
        public float ZPos;

        public float XVel;
        public float YVel;
        public float ZVel;

        public float Width;
        public float Height;

        public bool IsActive;

        private uint internalObjectType;
        public uint ObjectType
        {
            get
            {
                return internalObjectType;

            }
        }

        private static uint gameObjectCounter;
        private uint internalId;
        public uint Id
        {
            get
            {
                return internalId;
            }
        }

        public Room ownerRoom;
        

        public GameObject(uint objectType,Room ownerRoom)
        {
            internalObjectType = objectType;
            internalId = ++gameObjectCounter;

            this.ownerRoom = ownerRoom;
            //Console.WriteLine("spawned GameObject {0} of type {1}", Id, ObjectType);
            //add Gameobject in room      
        }

        public void SetPosition(float x, float y, float z)
        {
            XPos = x;
            YPos = y;
            ZPos = z;
        }

        public void SetVelocity(float x, float y, float z)
        {
            XVel = x;
            YVel = y;
            ZVel = z;
        }

        public virtual void Update()
        {
            XPos += XVel * ownerRoom.Server.CurrentClock.GetDeltaTime();
            YPos += YVel * ownerRoom.Server.CurrentClock.GetDeltaTime();
            ZPos += ZVel * ownerRoom.Server.CurrentClock.GetDeltaTime();

        }

        public virtual void OnCollide(Collision collisionInfo)
        {

        }
        
       
    }
}
