using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Essentials.Core.Databases
{
    public class DatabaseObject : ScriptableObject
    {
        public List<DatabaseItem> items = new List<DatabaseItem>();

        public DatabaseItem[] GetAllItems() => items.ToArray();

        public T[] GetAllItems<T>() where T : DatabaseItem
        {
            return items.Cast<T>().ToArray();
        }

        public DatabaseItem GetItem(string id) => items.Find(item => item.id == id);

        public T GetItem<T>(string id) where T : DatabaseItem
        {
            return items.Find(item => item.id == id) as T;
        }
    }
}