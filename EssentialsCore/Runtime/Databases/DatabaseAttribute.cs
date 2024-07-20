using System;
using UnityEngine;

namespace Essentials.Core.Databases
{
    public class DatabaseAttribute : Attribute
    {
        public Type databaseType { get; }

        public string newItemButtonLabel { get; } = "New Item";
        public string deleteItemButtonLabel { get; } = "Delete Item";

        public DatabaseAttribute(Type databaseType, string newButtonLabel = "New Item", string deleteButtonLabel = "Delete Item")
        {
            this.databaseType = databaseType;

            this.newItemButtonLabel = newButtonLabel;
            this.deleteItemButtonLabel = deleteButtonLabel;
        }
    }
}