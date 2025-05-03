using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Essentials.Core.Databases
{
    /// <summary>
    /// Represents a ScriptableObject database containing a list of DatabaseItems.
    /// Provides methods to retrieve items by ID or get all items.
    /// </summary>
    public class DatabaseObject : ScriptableObject
    {
        public List<DatabaseItem> items = new List<DatabaseItem>();

        /// <summary>
        /// Gets all items in the database as an array of DatabaseItem.
        /// </summary>
        /// <returns>An array containing all DatabaseItem objects in the database.</returns>
        public DatabaseItem[] GetAllItems() => items.ToArray();

        /// <summary>
        /// Gets all items in the database that are of a specific type T, derived from DatabaseItem.
        /// </summary>
        /// <typeparam name="T">The type of DatabaseItem to retrieve. Must inherit from DatabaseItem.</typeparam>
        /// <returns>An array containing all items of type T in the database.</returns>
        public T[] GetAllItems<T>() where T : DatabaseItem
        {
            return items.Cast<T>().ToArray();
        }

        /// <summary>
        /// Gets a specific item from the database by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the item to retrieve.</param>
        /// <returns>The DatabaseItem with the matching ID, or null if not found.</returns>
        public DatabaseItem GetItem(string id) => items.Find(item => item.id == id);

        /// <summary>
        /// Gets a specific item of type T from the database by its unique ID.
        /// </summary>
        /// <typeparam name="T">The type of DatabaseItem to retrieve. Must inherit from DatabaseItem.</typeparam>
        /// <param name="id">The unique identifier of the item to retrieve.</param>
        /// <returns>The DatabaseItem of type T with the matching ID, or null if not found or if the found item is not of type T.</returns>
        public T GetItem<T>(string id) where T : DatabaseItem
        {
            return items.Find(item => item.id == id) as T;
        }
    }
}