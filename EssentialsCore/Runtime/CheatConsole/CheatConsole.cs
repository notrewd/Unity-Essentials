using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Essentials.Core.CheatConsole
{
    [DisallowMultipleComponent]
    public class CheatConsole : MonoBehaviour
    {
        public static CheatConsole instance { get; private set; }
        
        [Header("Settings")]
        [Range(1, 1024)] public int maxLogs = 256;
        [SerializeField] private ConsoleCommandDatabase database;
        public bool autocomplete = true;

        [Header("Customization")]
        public string title = "Cheat Console";
        [SerializeField] private Color color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        public Padding padding;
        
        [Header("Keybinds")]
        [SerializeField] private InputActionReference openAction;
        [SerializeField] private InputActionAsset[] inputsToDisable;

        [Header("Default Commands")]
        [SerializeField] private bool helpCommand = true;
        [SerializeField] private bool printCommand = true;
        [SerializeField] private bool clearCommand = true;

        private ConsoleCommandDatabaseData commandDatabaseData;
        private bool consoleVisible;
        private CursorLockMode previousCursorLockMode;

        private Vector2 scrollPosition;
        private string currentCommand;

        private Queue<Log> logs = new Queue<Log>();
        private string[] cmdParts = Array.Empty<string>();

        private Texture2D backgroundTexture;
        private Texture2D foregroundTexture;

        private InputAction toggleAction;

        [Serializable]
        public struct Padding
        {
            public float top;
            public float bottom;
            public float left;
            public float right;
        }
        
        private struct Log
        {
            public string text;
            public LogFormat format;
        }
        
        public enum LogFormat { Info, Warning, Error }

        private void Awake()
        { 
            instance = this;

            if (database == null)
            {
                Debug.LogError("Essentials Cheat Console: Command database is null.");
                commandDatabaseData = new ConsoleCommandDatabaseData();
            }
            else commandDatabaseData = database.GetData();

            if (helpCommand)
            {
                ConsoleCommand command = new ConsoleCommand
                {
                    name = "help",
                    description = "Displays list of all possible commands.",
                    autocompleteEnabled = true,
                    validated = false,
                    arguments = new List<string> {"optional: parameters"}
                };

                commandDatabaseData.commands.Add(command);
            }

            if (printCommand)
            {
                ConsoleCommand command = new ConsoleCommand
                {
                    name = "print",
                    description = "Prints a text to the console.",
                    autocompleteEnabled = true,
                    arguments = new List<string> {"text"},
                    enabled = true,
                    validated = false
                };

                commandDatabaseData.commands.Add(command);
            }

            if (clearCommand)
            {
                ConsoleCommand command = new ConsoleCommand
                {
                    name = "clear",
                    description = "Clears the console logs.",
                    autocompleteEnabled = true,
                    arguments = new List<string>(),
                    enabled = true,
                    validated = true
                };

                commandDatabaseData.commands.Add(command);
            }
        }

        private void Start()
        {
            backgroundTexture = EssentialsUtility.SingleTexture2DColor(color);
            foregroundTexture = EssentialsUtility.SingleTexture2DColor(new Color(0.8f, 0.8f, 0.8f, 0.5f));

            if (openAction != null) toggleAction = openAction.action;
            else
            {
                InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
                InputActionMap map = asset.AddActionMap("Default");
                
                map.AddAction("Toggle", binding: "<Keyboard>/backquote");

                toggleAction = asset.FindAction("Toggle");
            }
            
            toggleAction.Enable();

            if (helpCommand) FindCommand("help").onExecute.AddListener(HelpCommand);
            if (printCommand) FindCommand("print").onExecuteRaw.AddListener(PrintCommand);
            if (clearCommand) FindCommand("clear").onExecute.AddListener(ClearCommand);
        }

        private void OnGUI()
        {
            if (!consoleVisible) return;
            
            GUI.Window(52, new Rect(padding.left, padding.top, Screen.width - padding.left - padding.right, Screen.height - padding.top - padding.bottom), OnWindow, "", new GUIStyle
            {
                normal =
                {
                    background = backgroundTexture
                }
            });
        }

        private void OnWindow(int id)
        {
            cmdParts = !string.IsNullOrWhiteSpace(currentCommand) ? currentCommand.Trim().Split(" ") : Array.Empty<string>();
            
            GUILayout.Label(title, new GUIStyle
            {
                normal =
                {
                    textColor = Color.white,
                    background = backgroundTexture
                },
                
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 20,
                fontStyle = FontStyle.Bold
            });

            GUI.skin.verticalScrollbar.normal.background = new Texture2D(0, 0);
            GUI.skin.verticalScrollbarThumb.normal.background = foregroundTexture;
            GUI.skin.verticalScrollbarUpButton = GUIStyle.none;
            GUI.skin.verticalScrollbarDownButton = GUIStyle.none;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUIStyle
            {
                padding = new RectOffset(10, 10, 5, 0)
            });
            foreach (Log log in logs)
            {
                switch (log.format)
                {
                    case LogFormat.Info:
                        GUILayout.Label(log.text, new GUIStyle { normal = { textColor = Color.white }, wordWrap = true});
                        break;
                    case LogFormat.Warning:
                        GUILayout.Label(log.text, new GUIStyle { normal = { textColor = new Color(1, 0.47f, 0.2f) }, wordWrap = true});
                        break;
                    case LogFormat.Error:
                        GUILayout.Label(log.text, new GUIStyle { normal = { textColor = Color.red }, wordWrap = true});
                        break;
                }
            }
            GUILayout.EndScrollView();

            HandleExecution();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(">", new GUIStyle
            {
                normal =
                {
                    textColor = Color.white
                },
                
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 30,
                fixedWidth = 10,
                padding = new RectOffset(10, 0, 0, 0)
            });
            
            HandleAutocomplete();
            
            GUI.SetNextControlName("commandInput");
            currentCommand = GUILayout.TextField(currentCommand, new GUIStyle
            {
                normal =
                {
                    textColor = Color.white
                },
                
                wordWrap = true,
                richText = false,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(15, 10, 0, 0)
            });
            GUILayout.EndHorizontal();
        }

        private void HandleExecution()
        {
            if (!Event.current.Equals(Event.KeyboardEvent("return"))|| GUI.GetNameOfFocusedControl() != "commandInput") return;
            if (string.IsNullOrWhiteSpace(currentCommand)) return;
            
            string cmdName = cmdParts[0];

            ConsoleCommand command = commandDatabaseData.commands.FirstOrDefault(x => x.enabled && x.name == cmdName);

            if (command == null)
            {
                ShowLog("Invalid command.", LogFormat.Error);
                return;
            }

            if (command.validated && command.arguments.Count != cmdParts.Length - 1)
            {
                if (command.arguments.Count == 0) ShowLog($"Command {command.name} doesn't accept any arguments.", LogFormat.Error);
                else ShowLog($"Command {command.name} accepts {command.arguments.Count} {(command.arguments.Count == 1 ? "argument" : "arguments")}, but was invoked with {cmdParts.Length - 1} {(cmdParts.Length - 1 == 1 ? "argument" : "arguments")}. Usage: {command.name} {string.Join(" ", command.arguments.Select(x => $"<{x}>"))}", LogFormat.Error);
                return;
            }
                
            command.onExecuteRaw.Invoke(cmdParts.Length - 1 == 0 ? string.Empty : string.Join(" ", cmdParts[1..]));
            command.onExecute.Invoke(cmdParts.Length - 1 == 0 ? Array.Empty<string>() : cmdParts[1..]);
        }

        private void HandleAutocomplete()
        {
            if (!autocomplete || cmdParts.Length == 0) return;
            
            string cmdName = cmdParts[0];

            ConsoleCommand foundCommand = commandDatabaseData.commands.FirstOrDefault(x => x.enabled && x.autocompleteEnabled && x.name.Length >= cmdName.Length && x.name[..cmdName.Length] == cmdName);

            if (foundCommand == null) return;

            string autocompleteText;

            if (cmdParts.Length > 1)
            {
                if (cmdParts.Length - 1 <= foundCommand.arguments.Count)
                {
                    int argumentNumber = cmdParts.Length - 1;
                    int charCount = cmdParts.Sum(x => x.Length + 1) - 1;

                    autocompleteText = $"{currentCommand[..charCount]} {string.Join(" ", foundCommand.arguments.GetRange(argumentNumber, foundCommand.arguments.Count - argumentNumber).Select(x => $"<{x}>"))}";
                }
                else autocompleteText = currentCommand;
            }
            else autocompleteText = $"{foundCommand.name} {string.Join(" ", foundCommand.arguments.Select(x => $"<{x}>"))}";

            GUI.Label(GUILayoutUtility.GetLastRect(), autocompleteText, new GUIStyle
            {
                normal =
                {
                    textColor = Color.gray
                },

                wordWrap = true,
                richText = false,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(25, 10, 0, 0)
            });

            if (!Event.current.Equals(Event.KeyboardEvent("tab")) || GUI.GetNameOfFocusedControl() != "commandInput" || cmdParts.Length > 1) return;

            currentCommand = foundCommand.arguments.Count == 0 ? foundCommand.name : foundCommand.name + " ";

            TextEditor editor = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

            StartCoroutine(MoveLineToEnd(editor));
        }

        private IEnumerator MoveLineToEnd(TextEditor editor)
        {
            yield return null;
            editor.MoveLineEnd();
        }
        
        private void Update()
        {
            if (!toggleAction.WasPressedThisFrame()) return;
            
            if (!consoleVisible)
            {
                previousCursorLockMode = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;
                foreach (InputActionAsset asset in inputsToDisable) asset.Disable();
            }
            else
            {
                Cursor.lockState = previousCursorLockMode;
                foreach (InputActionAsset asset in inputsToDisable) asset.Enable();
            }
                
            consoleVisible = !consoleVisible;
        }

        #region Default Commands

        private void HelpCommand(string[] args)
        {
            ConsoleCommand[] allCommands = GetAllCommands();
            
            if (args.Length == 0)
            {
                int maxPages = allCommands.Length / 10 + (allCommands.Length % 10 == 0 ? 0 : 1);
                
                ShowLog("List of commands:");
                
                ConsoleCommand[] commands = allCommands.Length <= 10 ? allCommands : allCommands[..10];

                foreach (ConsoleCommand command in commands) ShowLog($"{command.name}  |  {command.description}");
                
                ShowLog("--------------------------");
                ShowLog($"Page 1 of {maxPages}");
                ShowLog("<i><color=grey>To display more pages use \"help -p <pageNumber>\".</color></i>");
            }
            else if (args.Length == 2)
            {
                if (args[0] == "-p" && int.TryParse(args[1], out int page))
                {
                    int maxPages = allCommands.Length / 10 + (allCommands.Length % 10 == 0 ? 0 : 1);
                    
                    if (page < 1 || page > maxPages)
                    {
                        ShowLog("Invalid page number.", LogFormat.Error);
                        return;
                    }
                    
                    ShowLog("List of commands:");

                    ConsoleCommand[] commands = allCommands[((page - 1) * 10)..].Length <= 10 ? allCommands[((page - 1) * 10)..] : allCommands[((page - 1) * 10)..(page * 10)];

                    foreach (ConsoleCommand command in commands) ShowLog($"{command.name}  |  {command.description}");
                    
                    ShowLog("--------------------------");
                    ShowLog($"Page {page} of {maxPages}");
                }
                else ShowLog("Invalid command parameters.", LogFormat.Error);
            }
            else ShowLog("Invalid command parameters.", LogFormat.Error);
        }

        private void PrintCommand(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ShowLog("Print text can't be empty.", LogFormat.Error);
                return;
            }
            
            ShowLog(text);
        }

        private void ClearCommand(string[] args) => ClearLogs();

        #endregion

        #region Public Methods

        public void ShowLog(string text, LogFormat format = LogFormat.Info)
        {
            logs.Enqueue(new Log {text = format switch
            {
                LogFormat.Info => text,
                LogFormat.Warning => $"Warning: {text}",
                LogFormat.Error => $"Error: {text}",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            }, format = format});
            
            if (logs.Count > maxLogs) logs.Dequeue();

            scrollPosition = new Vector2(scrollPosition.x, Mathf.Infinity);
        }

        public void ClearLogs() => logs.Clear();

        public ConsoleCommand FindCommand(string commandName) => commandDatabaseData.commands.FirstOrDefault(x => x.name == commandName);

        public ConsoleCommand[] GetAllCommands() => commandDatabaseData.commands.Where(x => x.enabled && !x.hidden).ToArray();

        #endregion
    }
}