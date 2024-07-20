using System;
using UnityEngine;

namespace Essentials.Core.Databases
{
    public class DatabaseAttribute : Attribute
    {
        public Type DatabaseType { get; }

        public string NewItemButtonLabel { get; } = "New Item";
        public string DeleteItemButtonLabel { get; } = "Delete Item";

        public DatabaseAttribute(Type databaseType, string newButtonLabel = "New Item", string deleteButtonLabel = "Delete Item")
        {
            DatabaseType = databaseType;

            NewItemButtonLabel = newButtonLabel;
            DeleteItemButtonLabel = deleteButtonLabel;
        }
    }
}