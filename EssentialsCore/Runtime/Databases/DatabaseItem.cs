using UnityEngine;

namespace Essentials.Core.Databases
{
    /// <summary>
    /// Base class for items stored within a DatabaseObject.
    /// Each item must have a unique string ID.
    /// </summary>
    public class DatabaseItem : ScriptableObject
    {
        /// <summary>
        /// The unique identifier for this database item.
        /// </summary>
        public string id;
    }
}