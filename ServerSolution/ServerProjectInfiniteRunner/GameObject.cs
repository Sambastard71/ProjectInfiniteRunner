using System;
namespace ServerProjectInfiniteRunner
{
    public enum ObjectType
    {
        Avatar,
        ShortObstacle,
        NormalObstacle,
        TallObstacle
    }

    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }


    public abstract class GameObject
    {
        public Vector2 Position;

        public Vector2 Velocity;

        public float Width;
        public float Height;

        public bool IsActive;

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
        

        public GameObject(uint objectType,Room ownerRoom)
        {
            internalObjectType = (ObjectType)objectType;
            internalId = ++gameObjectCounter;

            this.ownerRoom = ownerRoom;
            //Console.WriteLine("spawned GameObject {0} of type {1}", Id, ObjectType);
            //add Gameobject in room      
        }

        public void SetPosition(float x, float y, float z)
        {
            Position.X = x;
            Position.Y = y;
        }

        public void SetVelocity(float x, float y, float z)
        {
            Velocity.X = x;
            Velocity.Y = y;
        }

        public virtual void Update()
        {
            Position.X += Velocity.X * ownerRoom.Server.CurrentClock.GetDeltaTime();
            Position.Y += Velocity.Y * ownerRoom.Server.CurrentClock.GetDeltaTime();
        }

        public virtual void OnCollide(Collision collisionInfo)
        {

        }
    }
}
