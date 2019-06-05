using System;
namespace ServerProjectInfiniteRunner
{
    public enum ObjectType
    {
        Avatar,
        OrizontalObstacle,
        VerticalRightObstacle,
        VerticalLeftObstacle
        
    }

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;


        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }


    public abstract class GameObject
    {
        public Vector3 Position;

        public Vector3 Velocity;

        protected float counterToUpdate = Server.TIME_TO_SEND_UPDATE;

        public float Width;
        public float Height;

        public bool IsActive;
        public bool isCollisionAffected;

        private ObjectType internalObjectType;
        public ObjectType ObjectType
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


        public GameObject(uint objectType, Room ownerRoom)
        {
            internalObjectType = (ObjectType)objectType;
            internalId = ++gameObjectCounter;

            IsActive = true;
            isCollisionAffected = true;

            this.ownerRoom = ownerRoom;
            //Console.WriteLine("spawned GameObject {0} of type {1}", Id, ObjectType);
            //add Gameobject in room      
        }

        public void SetPosition(float x, float y, float z)
        {
            Position.X = x;
            Position.Y = y;
            Position.Z = z;

        }

        public void SetVelocity(float x, float y, float z)
        {
            Velocity.X = x;
            Velocity.Y = y;
            Velocity.Z = z;

        }

        public virtual void Update()
        {
            

            Position.X += Velocity.X ;
            Position.Y += Velocity.Y ;
            Position.Z += Velocity.Z ;

        }

        public virtual void OnCollide(Collision collisionInfo)
        {

        }

        public virtual void SendUpdate()
        {

        }
    }
}