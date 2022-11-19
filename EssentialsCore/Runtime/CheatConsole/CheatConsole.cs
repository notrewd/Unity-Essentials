using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Essentials.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Essentials.Core.CheatConsole
{
    public class CheatConsole : MonoBehaviour
    {
        public static CheatConsole instance { get; private set; }
        
        [Range(1, 1024)] public int maxLogs = 256;
        [Range(1, 20)] public int autocompleteMaxSuggestions = 5;
        [SerializeField] private ConsoleCommandDatabase database;
        [Space]
        [SerializeField] private GameObject console;
        [SerializeField] private CanvasGroup consoleCanvasGroup;
        [SerializeField] private TMP_InputField cmdInputField;
        [SerializeField] private OptimizedScrollRect scrollRect;
        [SerializeField] private TMP_Text logPrefab;
        [SerializeField] private TMP_Text autocompleteSuggestionPrefab;
        [SerializeField] private GameObject autocomplete;
        [SerializeField] private GameObject consolePrefab;

        private Queue<TMP_Text> logs = new Queue<TMP_Text>();
        private List<string> queuedLogs = new List<string>();
        private ConsoleCommandDatabaseData commandDatabaseData;
        private ConsoleCommand[] autocompleteFoundCommands;
        private int autocompleteIndex = 0;
        private bool consoleVisible;
        private bool logIsBusy;

        private float animCurrentAlpha;
        private float animTime;
        private float animSpeed;

        private void Awake()
        { 
            instance = this;

            if (database == null)
            {
                Debug.LogError("Essentials Cheat Console: Command database is null.");
                commandDatabaseData = new ConsoleCommandDatabaseData();
            }
            else commandDatabaseData = database.GetData();
        }

        private void Start()
        {
            cmdInputField.onSubmit.AddListener(OnSubmit);
            cmdInputField.onValueChanged.AddListener(Autocomplete);
            
            console.SetActive(false);
            consoleCanvasGroup.alpha = 0;
        }

        private void Update()
        {
            if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                consoleVisible = !consoleVisible;

                if (consoleVisible)
                {
                    console.SetActive(true);
                    StartCoroutine(DisplayQueuedLogs());
                }
                
                animCurrentAlpha = consoleCanvasGroup.alpha;
                animTime = 0;
                animSpeed = 10;
            }

            if (animTime < 1)
            {
                animTime += Time.deltaTime * animSpeed;
                consoleCanvasGroup.alpha = consoleVisible ? Mathf.Lerp(animCurrentAlpha, 1, animTime) : Mathf.Lerp(animCurrentAlpha, 0, animTime);
            }
            else if (console.activeSelf != consoleVisible) console.SetActive(consoleVisible);

            if (!consoleVisible) return;
            
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (!autocomplete.activeSelf) return;
                cmdInputField.text = autocompleteFoundCommands[autocompleteIndex].name;
                cmdInputField.caretPosition = cmdInputField.text.Length;
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame && autocomplete.activeSelf)
            {
                if (autocompleteIndex == 0) autocompleteIndex = autocompleteFoundCommands.Length - 1;
                else autocompleteIndex--;
                cmdInputField.caretPosition = cmdInputField.text.Length;
                
                Autocomplete(cmdInputField.text);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame && autocomplete.activeSelf)
            {
                if (autocompleteIndex == autocompleteFoundCommands.Length - 1) autocompleteIndex = 0;
                else autocompleteIndex++;
                cmdInputField.caretPosition = cmdInputField.text.Length;

                Autocomplete(cmdInputField.text);
            }
        }

        private void Autocomplete(string command)
        {
            string[] cmdParts = command.Split(" ");
            string cmdName = cmdParts[0];

            if (string.IsNullOrEmpty(cmdName))
            {
                autocomplete.SetActive(false);
                return;
            }
            
            autocompleteFoundCommands = commandDatabaseData.commands.Where(x => x.enabled && x.autocompleteEnabled && x.name[..Mathf.Clamp(cmdName.Length, 1, x.name.Length)] == cmdName).ToArray();

            if (cmdParts.Length >= 2) autocompleteFoundCommands = autocompleteFoundCommands.Where(x => x.name == cmdName).ToArray();

            if (autocompleteFoundCommands.Length != 0)
            {
                if (cmdParts.Length >= 2 && autocompleteFoundCommands.Select(x => x.name).All(x => x != cmdName))
                {
                    autocomplete.SetActive(false);
                    return;
                }
                
                autocomplete.SetActive(true);

                if (autocompleteIndex >= autocompleteFoundCommands.Length) autocompleteIndex = 0;
                
                foreach (Transform child in autocomplete.transform) Destroy(child.gameObject);
                for (int i = 0; i < Mathf.Clamp(autocompleteFoundCommands.Length, 1, autocompleteMaxSuggestions); i++)
                {
                    TMP_Text suggestion = Instantiate(autocompleteSuggestionPrefab, autocomplete.transform, false);
                    string[] arguments = new string[autocompleteFoundCommands[i].arguments.Count];

                    for (int j = 0; j < autocompleteFoundCommands[i].arguments.Count; j++)
                    {
                        string argument = autocompleteFoundCommands[i].arguments[j];
                        arguments[j] = cmdParts.Length - 2 >= j ? $"<color=yellow><{argument}></color>" : $"<{argument}>";
                    }

                    suggestion.text = $"<color=yellow>{autocompleteFoundCommands[i].name[..cmdName.Length]}</color>{autocompleteFoundCommands[i].name[cmdName.Length..]} {string.Join(" ", arguments)}";
                    if (i == autocompleteIndex) suggestion.text = "> " + suggestion.text;
                }
            }
            else autocomplete.SetActive(false);
        }
        
        private void OnSubmit(string commandText)
        {
            string[] cmdParts = commandText.Split(" ");
            string cmdName = cmdParts[0];
            string cmdArgumentsRaw = cmdParts.Length >= 2 ? commandText[(cmdName.Length + 1)..] : string.Empty;
            string[] cmdArguments = cmdArgumentsRaw.Split(" ");

            if (string.IsNullOrEmpty(cmdName)) return;

            autocomplete.SetActive(false);
            
            ConsoleCommand command = commandDatabaseData.commands.Where(x => x.enabled).FirstOrDefault(x => x.name == cmdName);

            if (command == null)
            {
                ShowLogError("Invalid command.");
                return;
            }
            if (command.validated && command.arguments.Count(x => !string.IsNullOrWhiteSpace(x))!= cmdArguments.Count(x => !string.IsNullOrWhiteSpace(x)))
            {
                string validArguments = command.arguments.Count(x => !string.IsNullOrWhiteSpace(x)) != 0 ? string.Join(", ", command.arguments.Select(x => $"<{x}>").ToArray()) : "none";
                ShowLogError($"Invalid arguments for {command.name}. Valid arguments are: {validArguments}.");
                return;
            }
            
            command.onExecuteRaw.Invoke(cmdArgumentsRaw);
            command.onExecute.Invoke(cmdArguments);
        }

        private void RegisterLog(TMP_Text log)
        {
            scrollRect.EnableRecalculationOnScroll(false);
            scrollRect.Add(log.gameObject, OptimizedScrollRect.RecalculateMode.ContentSize);
            scrollRect.Recalculate(OptimizedScrollRect.RecalculateMode.Layout);
            logs.Enqueue(log);
            if (logs.Count > maxLogs)
            {
                scrollRect.Remove(logs.Dequeue().gameObject, OptimizedScrollRect.RecalculateMode.Layout);
                scrollRect.Recalculate(OptimizedScrollRect.RecalculateMode.ContentSize);
            }
        }

        private IEnumerator ShowLogCoroutine(string text)
        {
            logIsBusy = true;
            TMP_Text log = Instantiate(logPrefab);
            log.text = text;
            RegisterLog(log);

            yield return null;

            while (log.isTextOverflowing)
            {
                TMP_Text linkedLog = Instantiate(logPrefab);
                log.overflowMode = TextOverflowModes.Linked;
                log.linkedTextComponent = linkedLog;
                log = linkedLog;
                RegisterLog(log);

                yield return null;
            }
            
            scrollRect.Recalculate(OptimizedScrollRect.RecalculateMode.Visibility);
            scrollRect.EnableRecalculationOnScroll(true);
            logIsBusy = false;
        }

        private IEnumerator DisplayQueuedLogs()
        {
            if (queuedLogs.Count == 0) yield break;

            while (logIsBusy) yield return null;

            foreach (string queuedLog in queuedLogs) yield return StartCoroutine(ShowLogCoroutine(queuedLog));
            
            queuedLogs.Clear();
        }

        public void ShowLog(string text)
        {
            if (logIsBusy) return;
            if (!scrollRect.gameObject.activeInHierarchy)
            {
                queuedLogs.Add(text);
                return;
            }
            StartCoroutine(ShowLogCoroutine(text));
        }

        public void ShowLogError(string text) => ShowLog($"<color=red>{text}</color>");

        public void ClearLogs()
        {
            logs.Clear();
            scrollRect.Clear();
        }

        public ConsoleCommand FindCommand(string commandName) => commandDatabaseData.commands.FirstOrDefault(x => x.name == commandName);

        public ConsoleCommand[] GetAllCommands() => commandDatabaseData.commands.Where(x => !x.hidden).ToArray();
    }
}