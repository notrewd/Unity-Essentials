using System;
using Essentials.Core.Databases;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Essentials.Internal.Databases
{
    public class DatabaseEditor : EditorWindow
    {
        private static void CreateWindow<T>() where T : DatabaseObject<T>
        {
            DatabaseEditor window = CreateInstance<DatabaseEditor>();
            window.titleContent = new GUIContent("Database Editor");
            window.minSize = new Vector2(300, 300);
            window.Show();
        }

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);

            if (!obj.GetType().BaseType.IsGenericType) return false;

            if (obj.GetType().BaseType.GetGenericTypeDefinition() == typeof(DatabaseObject<>))
            {
                return true;
            }

            return false;
        }
    }
}
