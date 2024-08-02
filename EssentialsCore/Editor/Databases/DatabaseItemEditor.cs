using Essentials.Core.Databases;
using UnityEditor;
using UnityEngine;

namespace Essentials.Internal.Databases
{
    [CustomEditor(typeof(DatabaseItem), true)]
    public class DatabaseItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.GetIterator();

            while (property.NextVisible(true))
            {
                if (property.name == "m_Script") continue;

                if (property.name == "id")
                {
                    EditorGUILayout.PropertyField(property, new GUIContent("ID"));
                    GUILayout.Space(5f);
                    serializedObject.ApplyModifiedProperties();
                    continue;
                }

                EditorGUILayout.PropertyField(property);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
