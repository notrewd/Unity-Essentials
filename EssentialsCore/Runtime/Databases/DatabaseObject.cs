using System.Collections.Generic;
using UnityEngine;

namespace Essentials.Core.Databases
{
    public class DatabaseObject : ScriptableObject
    {
        public List<DatabaseItem> items = new List<DatabaseItem>();

        public DatabaseItem[] GetAllItems() => items.ToArray();
        public DatabaseItem GetItem(string id) => items.Find(item => item.id == id);
    }
}