using System.Linq;
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
            DatabaseObject databaseObject = target as DatabaseObject;

            if (databaseObject == null) return;

            int itemCount = databaseObject.items.Count;
            int itemsWithNoId = databaseObject.items.FindAll(x => string.IsNullOrEmpty(x.id)).Count;
            int itemsWithDuplicateId = databaseObject.items.GroupBy(x => x.id).Count(x => x.Count() > 1);

            GUILayout.Label($"{target.name} Details", EditorStyles.boldLabel);
            GUILayout.Space(5f);
            GUILayout.Label($"Item Count: {itemCount}");

            if (itemsWithNoId > 0)
            {
                GUILayout.BeginHorizontal();

                GUI.color = Color.red;
                GUILayout.Label($"Items With No ID: {itemsWithNoId}");
                GUI.color = Color.white;

                if (GUILayout.Button("Fix", GUILayout.Width(50f)))
                {
                    for (int i = 0; i < databaseObject.items.Count; i++)
                    {
                        if (string.IsNullOrEmpty(databaseObject.items[i].id))
                        {
                            DatabaseEditor.GenerateIdForItem(databaseObject.items[i], databaseObject);
                        }
                    }
                }

                GUILayout.EndHorizontal();
            }

            if (itemsWithDuplicateId > 0)
            {
                GUILayout.BeginHorizontal();

                GUI.color = Color.red;
                GUILayout.Label($"Items With Duplicate ID: {itemsWithDuplicateId}");
                GUI.color = Color.white;

                if (GUILayout.Button("Fix", GUILayout.Width(50f)))
                {
                    for (int i = 0; i < databaseObject.items.Count; i++)
                    {
                        if (databaseObject.items.GroupBy(x => x.id).Count(x => x.Count() > 1) > 0)
                        {
                            DatabaseEditor.GenerateIdForItem(databaseObject.items[i], databaseObject);
                        }
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);
            if (GUILayout.Button("Open Database")) DatabaseEditor.CreateWindow(target as DatabaseObject);
        }
    }
}
