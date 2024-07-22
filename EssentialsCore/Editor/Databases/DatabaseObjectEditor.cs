using Essentials.Core.Databases;
using UnityEditor;
using UnityEngine;

namespace Essentials.Internal.Databases
{
    [CustomEditor(typeof(DatabaseObject), true)]
    public class DatabaseObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            int itemCount = ((DatabaseObject)target).items.Count;
            int itemsWithNoId = ((DatabaseObject)target).items.FindAll(x => string.IsNullOrEmpty(x.id)).Count;

            GUILayout.Label($"{target.name} Details", EditorStyles.boldLabel);
            GUILayout.Space(5f);
            GUILayout.Label($"Item Count: {itemCount}");

            if (itemsWithNoId > 0)
            {
                GUI.color = Color.red;
                GUILayout.Label($"Items With No ID: {itemsWithNoId}");
                GUI.color = Color.white;
            }

            GUILayout.Space(5f);
            if (GUILayout.Button("Open Database")) DatabaseEditor.CreateWindow(target as DatabaseObject);
        }
    }
}
