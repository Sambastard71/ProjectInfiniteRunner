using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    static class UpdateManager
    {
        public enum ColliderType : uint { Player = 1, Obstacle = 2}

        static List<IUpdatable> items;
        static List<IUpdatable> itemsToRemove;

        static Collision collisionInfo;

        static UpdateManager()
        {
            items = new List<IUpdatable>();
            itemsToRemove = new List<IUpdatable>();
        }

        public static void AddItem(IUpdatable item)
        {
            items.Add(item);
        }

        public static void AddItemToRemoveList(IUpdatable item)
        {
            itemsToRemove.Add(item);
        }

        public static void RemoveItem(IUpdatable item)
        {
            items.Remove(item);
        }

        public static void RemoveAll()
        {
            items.Clear();
        }

        public static void Update()
        {
            if(items.Count>0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Update();
                }

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SendUpdate();                
                }
            }
        }

        public static void CheckCollisions()
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                if (items[i].GetIsActive() && items[i].GetIsCollisionAffected())
                {
                    for (int j = i + 1; j < items.Count; j++)
                    {
                        if (items[j].GetIsActive() && items[j].GetIsCollisionAffected())
                        {
                            bool checkFirst = items[i].CheckCollisionWith(items[j].GetCollider());
                            bool checkSecond = items[j].CheckCollisionWith(items[i].GetCollider());

                            if (items[i].GetRoomId() == items[j].GetRoomId() && (checkFirst || checkSecond) && items[i].GetCollider().Collides(items[j].GetCollider(), ref collisionInfo))
                            {
                                if (checkFirst)
                                {
                                    collisionInfo.Collider = items[j].GetGameObject();
                                    items[i].GetGameObject().OnCollide(collisionInfo);
                                }
                                if (checkSecond)
                                {
                                    collisionInfo.Collider = items[i].GetGameObject();
                                    items[j].GetGameObject().OnCollide(collisionInfo);
                                }

                                foreach (IUpdatable item in itemsToRemove)
                                {
                                    RemoveItem(item);
                                }

                                itemsToRemove.RemoveAll(ToDelete);
                            }
                        }
                    }
                }
            }
        }

        private static bool ToDelete(IUpdatable item)
        {
            return true;
        }

    }
}
