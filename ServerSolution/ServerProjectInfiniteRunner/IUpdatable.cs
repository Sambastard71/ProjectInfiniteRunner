using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    public interface IUpdatable
    {
        bool GetIsCollisionAffected();
        void SetIsCollisionAffected(bool boolean);

        bool GetIsActive();
        void SetIsActive(bool boolean);

        uint GetRoomId();

        bool CheckCollisionWith(Collider2D collider);
        Collider2D GetCollider();

        GameObject GetGameObject();

        void Update();

        void SendUpdate();
    }
}
