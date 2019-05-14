using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner
{
    static class SpawnManager
    {
        static List<ISpawnable> items;

        static SpawnManager()
        {
            items = new List<ISpawnable>();
        }

        public static void AddItem(ISpawnable item)
        {
            items.Add(item);
        }

        public static void RemoveItem(ISpawnable item)
        {
            items.Remove(item);
        }

        public static void RemoveAll()
        {
            items.Clear();
        }

        public static void Spawn()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Spawn();
            }
        }
    }
}
