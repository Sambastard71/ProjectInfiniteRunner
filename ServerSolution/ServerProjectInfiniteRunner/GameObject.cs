using System;
namespace ServerProjectInfiniteRunner
{
    public abstract class GameObject
    {
        public float X;
        public float Y;
        public float Z;

        

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

        private uint roomId;
        public uint RoomId
        {
            get
            {
                return roomId;
            }
        }

        public GameObject(uint objectType)
        {
            internalObjectType = objectType;
            internalId = ++gameObjectCounter;
            //add roomId
            //add Gameobject in room      
        }

        public void SetPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public virtual void Tick()
        {

        }
    }
}
