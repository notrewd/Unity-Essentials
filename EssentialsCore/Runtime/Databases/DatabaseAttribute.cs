using System;

namespace Essentials.Core.Databases
{
    /// <summary>
    /// Attribute used to associate a DatabaseItem class with its corresponding DatabaseObject type.
    /// This is used by the editor tools to manage database items.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseAttribute : Attribute
    {
        /// <summary>
        /// Gets the Type of the DatabaseObject this item belongs to.
        /// </summary>
        public Type databaseType { get; }

        /// <summary>
        /// Gets the label used for displaying items of this type in the editor (e.g., "Weapon", "Character").
        /// </summary>
        public string itemLabel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAttribute"/> class.
        /// </summary>
        /// <param name="databaseType">The Type of the DatabaseObject this item class belongs to.</param>
        /// <param name="itemLabel">The label to use for items of this type in the editor. Defaults to "Item".</param>
        public DatabaseAttribute(Type databaseType, string itemLabel = "Item")
        {
            this.databaseType = databaseType;
            this.itemLabel = itemLabel;
        }
    }
}