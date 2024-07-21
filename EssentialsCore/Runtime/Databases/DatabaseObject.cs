using System.Collections.Generic;
using UnityEngine;

namespace Essentials.Core.Databases
{
    public class DatabaseObject : ScriptableObject
    {
        public List<DatabaseItem> items = new List<DatabaseItem>();
    }
}