using System;

namespace Essentials.Core.Databases
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseAttribute : Attribute
    {
        public Type databaseType { get; }

        public string itemLabel { get; }

        public DatabaseAttribute(Type databaseType, string itemLabel = "Item")
        {
            this.databaseType = databaseType;
            this.itemLabel = itemLabel;
        }
    }
}