using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Essentials.Core.CheatConsole
{
    public class ConsoleCommandDatabaseEditorWindow : EditorWindow
    {
        private static ConsoleCommandDatabase currentDatabase;
        private static ConsoleCommandDatabaseData currentDatabaseData;

        private Vector2 listScrollPosition;
        private Vector2 commandScrollPosition;
        private int selectedIndex;

        public static void OpenDatabase(ConsoleCommandDatabase database)
        {
            EditorWindow window = GetWindow<ConsoleCommandDatabaseEditorWindow>();
            window.titleContent = new GUIContent(database.name, EditorGUIUtility.IconContent("d_SceneViewTools").image);

            currentDatabase = database;
            currentDatabaseData = database.GetData();
        }

        private void OnGUI()
        {
            if (currentDatabase == null)
            {
                GUILayout.Label("There has been an error while getting the database. Please try again.", new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.gray
                    },
                    stretchHeight = true,
                    stretchWidth = true,
                    alignment = TextAnchor.MiddleCenter
                });

                return;
            }

            if (currentDatabaseData.commands.Count == 0)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                
                GUILayout.Label("You haven't created any commands yet.", new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.gray
                    },
                    stretchWidth = true,
                    alignment = TextAnchor.MiddleCenter
                });

                GUILayout.Space(4);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add New Command", GUILayout.Width(150))) AddNewCommand();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                
                return;
            }
            
            GUILayout.BeginHorizontal(new GUIStyle
            {
                padding = new RectOffset(10, 10, 10, 10)
            });

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(150));
            
            GUILayout.BeginVertical();
            
            listScrollPosition = GUILayout.BeginScrollView(listScrollPosition);

            for (int i = 0; i < currentDatabaseData.commands.Count; i++)
            {
                ConsoleCommand command = currentDatabaseData.commands[i];
                
                if (GUILayout.Button(new GUIContent(command.name, EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow").image), new GUIStyle
                {
                    normal =
                    {
                        textColor = currentDatabaseData.commands.IndexOf(command) == selectedIndex ? Color.yellow : Color.white
                    },
                    alignment = TextAnchor.MiddleCenter
                }))
                {
                    GUI.FocusControl(null);
                    selectedIndex = i;
                }

                if (i != currentDatabaseData.commands.Count - 1) GUILayout.Space(5);
            }
            
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Add New Command")) AddNewCommand();

            GUILayout.EndVertical();
            
            GUILayout.Space(5);
            HorizontalLine(new Color(0.35f, 0.35f, 0.35f));
            GUILayout.Space(6);
            
            GUILayout.EndHorizontal();
            
            ConsoleCommand selectedCommand = currentDatabaseData.commands[selectedIndex];

            GUILayout.BeginVertical();
            
            commandScrollPosition = GUILayout.BeginScrollView(commandScrollPosition);
            
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(selectedCommand.name, new GUIStyle
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 20,
                fontStyle = FontStyle.Bold
            });
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);

            selectedCommand.name = StringField("Name", selectedCommand.name.Replace(" ", "_"));
            selectedCommand.description = MultilineStringField("Description", selectedCommand.description);
            selectedCommand.validated = Toggle("Validated", selectedCommand.validated);
            selectedCommand.autocompleteEnabled = Toggle("Autocomplete", selectedCommand.autocompleteEnabled);
            selectedCommand.hidden = Toggle("Hidden", selectedCommand.hidden);

            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Arguments");
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Toolbar Plus").image, "Add a new argument."), new GUIStyle
            {
                contentOffset = new Vector2(0, 1.5f)
            }, GUILayout.Width(20))) selectedCommand.arguments.Add("newArgument");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            VerticalLine(new Color(0.35f, 0.35f, 0.35f));
            GUILayout.Space(5);

            if (selectedCommand.arguments.Count != 0)
            {
                int argumentToRemove = -1;
                
                for (int i = 0; i < selectedCommand.arguments.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    selectedCommand.arguments[i] = EditorGUILayout.TextField(selectedCommand.arguments[i]);
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus").image, new GUIStyle
                    {
                        contentOffset = new Vector2(0, 1.5f)
                    }, GUILayout.Width(20))) argumentToRemove = i;
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(2);
                }

                if (argumentToRemove >= 0)
                {
                    GUI.FocusControl(null);
                    selectedCommand.arguments.RemoveAt(argumentToRemove);
                }
            }

            GUILayout.EndVertical();
            
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Remove Command", GUILayout.Width(130)))
            {
                GUI.FocusControl(null);
                currentDatabaseData.commands.RemoveAt(selectedIndex);
                selectedIndex = 0;
                ApplyData();
            }
            
            if (GUILayout.Button("Apply", GUILayout.Width(70)))
            {
                GUI.FocusControl(null);
                ApplyData();
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(2);
            
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        private void HorizontalLine(Color color)
        {
            Color previousColor = GUI.color;
            GUI.color = color;
            
            GUILayout.Box(GUIContent.none, new GUIStyle
            {
                stretchHeight = true,
                fixedWidth = 1,
                normal =
                {
                    background = EditorGUIUtility.whiteTexture
                }
            });

            GUI.color = previousColor;
        }
        
        private void VerticalLine(Color color)
        {
            Color previousColor = GUI.color;
            GUI.color = color;
            
            GUILayout.Box(GUIContent.none, new GUIStyle
            {
                fixedHeight = 1,
                stretchWidth = true,
                normal =
                {
                    background = EditorGUIUtility.whiteTexture
                }
            });

            GUI.color = previousColor;
        }

        private string StringField(string label, string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(150));
            text = EditorGUILayout.TextField(text);
            GUILayout.EndHorizontal();

            return text;
        }

        private bool Toggle(string label, bool toggle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(150));
            toggle = EditorGUILayout.Toggle(toggle, GUILayout.Width(15));
            GUILayout.EndHorizontal();
            return toggle;
        }
        
        private string MultilineStringField(string label, string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(150));
            text = EditorGUILayout.TextArea(text);
            GUILayout.EndHorizontal();

            return text;
        }

        private void AddNewCommand()
        {
            ConsoleCommand newCommand = new ConsoleCommand
            {
                name = "new_command",
                description = "No description.",
                validated = true,
                autocompleteEnabled = true
            };
            
            currentDatabaseData.commands.Add(newCommand);
                
            ApplyData();
        }

        private void ApplyData()
        {
            SerializedObject serializedObject = new SerializedObject(currentDatabase);
            serializedObject.FindProperty("data").stringValue = JsonUtility.ToJson(currentDatabaseData);
            serializedObject.ApplyModifiedProperties();
        }
    }
}