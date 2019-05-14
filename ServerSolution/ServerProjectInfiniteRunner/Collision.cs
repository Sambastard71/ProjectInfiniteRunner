using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    
        public struct Collision
        {
            public enum CollisionType { None, RectsIntersection }

            public CollisionType Type;
            public float DeltaX;
            public float DeltaY;
            public GameObject Collider;
        }
    
}
