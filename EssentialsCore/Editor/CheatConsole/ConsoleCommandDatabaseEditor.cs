using UnityEditor;
using UnityEngine;

namespace Essentials.Core.CheatConsole
{
    [CustomEditor(typeof(ConsoleCommandDatabase))]
    public class ConsoleCommandDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ConsoleCommandDatabase database = (ConsoleCommandDatabase) target;
            
            if (GUILayout.Button("Open Database")) ConsoleCommandDatabaseEditorWindow.OpenDatabase(database);

            if (string.IsNullOrEmpty(database.data)) return;
            
            if (database.data.Length > 1500)
            {
                GUILayout.Label($"{database.data.Remove(1500)}...", new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.gray
                    },
                    wordWrap = true,
                });
            }
            else
            {
                GUILayout.Label(database.data, new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.gray
                    },
                    wordWrap = true
                });
            }
        }
    }
}