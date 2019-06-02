using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerProjectInfiniteRunner
{
    public class Collider2D
    {
        public GameObject OwnerGameObject;

        public Vector3 Position
        {
            get { return OwnerGameObject.Position; }
        }
        
        public float Width
        {
            get { return OwnerGameObject.Width; }
        }

        public float Height
        {
            get { return OwnerGameObject.Height; }
        }

        public float HalfWidth;
        public float HalfHeight;

        public uint CollisionType;
        public uint CollisionMask;

        public Collider2D(GameObject gameObject)
        {
            OwnerGameObject = gameObject;
            HalfHeight = Height / 2;
            HalfWidth = Width / 2;
        }

        public bool Collides(Collider2D collider, ref Collision collisionInfo)
        {
            float distanceX = collider.Position.X - Position.X;
            float distanceZ = collider.Position.Z - Position.Z;

            float deltaX = Math.Abs(distanceX) - (HalfWidth + collider.HalfWidth);
            float deltaZ = Math.Abs(distanceZ) - (HalfHeight + collider.HalfHeight);

            if (deltaX <= 0 && deltaZ <= 0)
            {
                //setting collision's info
                collisionInfo.Type = Collision.CollisionType.RectsIntersection;
                collisionInfo.DeltaX = -deltaX; 
                collisionInfo.DeltaY = -deltaZ;
                return true;
            }
            return false;
        }

        public void AddCollision(uint mask)
        {
            CollisionMask |= mask;
        }
    }
}
