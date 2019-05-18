using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerProjectInfiniteRunner
{
    public class Collider2D
    {
        public Avatar OwnerAvatar;

        public float Xpos
        {
            get { return OwnerAvatar.XPos; }
        }

        public float Ypos
        {
            get { return OwnerAvatar.YPos; }
        }

        public float Width
        {
            get { return OwnerAvatar.Width; }
        }

        public float Height
        {
            get { return OwnerAvatar.Height; }
        }

        public float HalfWidth;
        public float HalfHeight;

        public uint CollisionType;
        public uint CollisionMask;

        public Collider2D(Avatar avatar)
        {
            OwnerAvatar = avatar;
            HalfHeight = Height / 2;
            HalfWidth = Width / 2;
        }

        public bool Collides(Collider2D collider, ref Collision collisionInfo)
        {
            float distanceX = collider.Xpos - Xpos;
            float distanceY = collider.Ypos - Ypos;


            float deltaX = Math.Abs(distanceX) - (HalfWidth + collider.HalfWidth);
            float deltaY = Math.Abs(distanceY) - (HalfHeight + collider.HalfHeight);

            if (deltaX <= 0 && deltaY <= 0)
            {
                //setting collision's info
                collisionInfo.Type = Collision.CollisionType.RectsIntersection;
                collisionInfo.DeltaX = -deltaX; 
                collisionInfo.DeltaY = -deltaY;
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
