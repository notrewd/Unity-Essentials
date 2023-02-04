using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Essentials.Core.GameDirectories;
using System.Linq;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectoriesEditor : EditorWindow
    {
        public static bool appliedChanges = true;

        private bool regenerateClass = false;

        private ScrollView scrollView;
        private VisualElement topBar;
        private VisualElement bottomBar;
        private Button newDirectoryButton;
        private TextField newDirectoryField;
        private Button applyButton;
        private Button settingsButton;

        private GameDirectoriesSettingsEditor settingsEditor;

        [MenuItem("Essentials/Game Directories")]
        private static void ShowWindow()
        {
            EditorWindow window = GetWindow<GameDirectoriesEditor>();
            window.titleContent = new GUIContent("Game Directories", EditorGUIUtility.IconContent("d_Project").image);
            window.minSize = new Vector2(300, 300);
        }

        public void CreateGUI()
        {
            GameDirectoriesSettings.LoadData();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameDirectories/GameDirectoriesEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            scrollView = rootVisualElement.Q<ScrollView>("ScrollView");
            topBar = rootVisualElement.Q<VisualElement>("TopBar");
            bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            newDirectoryButton = topBar.Q<Button>("NewDirectoryButton");
            newDirectoryField = topBar.Q<TextField>("NewDirectoryField");
            applyButton = bottomBar.Q<Button>("ApplyButton");
            settingsButton = bottomBar.Q<Button>("SettingsButton");

            appliedChanges = true;
            applyButton.SetEnabled(false);
            newDirectoryButton.SetEnabled(false);

            newDirectoryField.RegisterValueChangedCallback(e => newDirectoryButton.SetEnabled(ValidatePath(e.newValue)));

            newDirectoryField.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Return && ValidatePath(newDirectoryField.value))
                {
                    CreateGameDirectory(newDirectoryField.value);
                    RefreshScrollView();
                    newDirectoryField.value = string.Empty;
                }
            });

            newDirectoryButton.clicked += () =>
            {
                if (!ValidatePath(newDirectoryField.value)) return;

                CreateGameDirectory(newDirectoryField.value);
                RefreshScrollView();
                newDirectoryField.value = string.Empty;
            };

            applyButton.clicked += Apply;
            settingsButton.clicked += ShowSettingsWindow;

            GameDirectoryData[] gameDirectoriesData = GameDirectoriesSettings.GetGameDirectoriesData();

            if (gameDirectoriesData != null)
            {
                foreach (GameDirectoryData gameDirectoryData in gameDirectoriesData)
                {
                    CreateGameDirectory(gameDirectoryData.path, gameDirectoryData.reference);
                }

                RefreshScrollView();
            }

            applyButton.SetEnabled(false);
            appliedChanges = true;
        }

        private void CreateGameDirectory(string path, string reference = null)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            string[] directories = Array.Empty<string>();

            if (path.Contains("\\")) path = path.Replace("\\", "/");
            if (path.Contains("/")) directories = path.Split('/');

            else directories = new string[] { path };

            if (directories.Length == 0) return;

            foreach (string directory in directories)
            {
                if (string.IsNullOrWhiteSpace(directory)) return;
                if (directory.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return;
            }

            void CreateSubDirectores(GameDirectory directory, string[] subDirectories, string newPath = "")
            {
                if (subDirectories.Length == 0)
                {
                    directory.reference = reference;
                    return;
                }

                if (string.IsNullOrEmpty(newPath)) newPath = directory.name;

                GameDirectory subDirectory = directory.subDirectories.Find(x => x.name == subDirectories[0]);

                if (subDirectory == null)
                {
                    subDirectory = new GameDirectory() { name = subDirectories[0], path = newPath + "/" + subDirectories[0] };
                    directory.subDirectories.Add(subDirectory);
                }

                CreateSubDirectores(subDirectory, subDirectories[1..], newPath + "/" + subDirectories[0]);
            }

            GameDirectory gameDirectory = Core.GameDirectories.GameDirectories.gameDirectories.Find(x => x.name == directories[0]);

            if (gameDirectory == null)
            {
                gameDirectory = new GameDirectory() { name = directories[0], path = directories[0] };
                Core.GameDirectories.GameDirectories.gameDirectories.Add(gameDirectory);
            }

            CreateSubDirectores(gameDirectory, directories[1..]);

            appliedChanges = false;
            applyButton.SetEnabled(true);
        }

        private void RemoveGameDirectory(GameDirectory directory)
        {
            GameDirectory parentDirectory = Core.GameDirectories.GameDirectories.FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);

            if (directory.reference != null) regenerateClass = true;

            if (parentDirectory != null) parentDirectory.subDirectories.Remove(directory);
            else Core.GameDirectories.GameDirectories.gameDirectories.Remove(directory);

            appliedChanges = false;
            applyButton.SetEnabled(true);
        }

        private void MoveGameDirectory(GameDirectory directory, string newParentPath)
        {
            if (string.IsNullOrEmpty(newParentPath)) newParentPath = "~";
            if (newParentPath.Contains("\\")) newParentPath = newParentPath.Replace("\\", "/");

            if (newParentPath[^1] == '/') newParentPath = newParentPath[..^1];

            if (newParentPath == directory.path) return;

            if (newParentPath.Length == 1 && newParentPath == ".") newParentPath = "~";
            else if (newParentPath.Length == 2 && (newParentPath == "~/" || newParentPath == "./")) newParentPath = "~";
            else if (newParentPath.Length > 2 && (newParentPath[..2] == "./" || newParentPath[..2] == "~/")) newParentPath = newParentPath[2..];

            if (directory.reference != null) regenerateClass = true;

            if (newParentPath == "~")
            {
                GameDirectory oldParentDirectory = Core.GameDirectories.GameDirectories.FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);

                if (oldParentDirectory != null)
                {
                    oldParentDirectory.subDirectories.Remove(directory);
                    Core.GameDirectories.GameDirectories.gameDirectories.Add(directory);
                    directory.path = directory.name;
                }

                RefreshScrollView();

                appliedChanges = false;
                applyButton.SetEnabled(true);
                return;
            }

            GameDirectory parentDirectory = Core.GameDirectories.GameDirectories.FindGameDirectory(newParentPath);

            if (parentDirectory != null)
            {
                parentDirectory.subDirectories.Add(directory);
                GameDirectory oldParentDirectory = Core.GameDirectories.GameDirectories.FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);
                if (oldParentDirectory != null) oldParentDirectory.subDirectories.Remove(directory);
                else Core.GameDirectories.GameDirectories.gameDirectories.Remove(directory);
                directory.path = newParentPath + "/" + directory.name;
            }

            appliedChanges = false;
            applyButton.SetEnabled(true);
        }

        // show context menu when right clicking on a directory
        private void ShowDirectoryContextMenu(GameDirectory directory, VisualElement element)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddDisabledItem(new GUIContent($"Directory: {directory.name}"));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("New Sub Directory"), false, () =>
            {
                newDirectoryField.value = directory.path + "/";
                newDirectoryField.Focus();
                newDirectoryField.SelectRange(directory.path.Length + 1, directory.path.Length + 1);
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Change Reference"), false, () =>
            {
                UnityEditor.PopupWindow.Show(element.worldBound, new GameDirectoriesPopup("Enter Reference Name", directory.reference, "Change", (string reference) =>
                {
                    if (reference == directory.reference) return;
                    if (!string.IsNullOrEmpty(reference) && !char.IsLetter(reference[0])) return;
                    foreach (char character in reference) if (!char.IsLetterOrDigit(character) && character != '_') return;
                    if (Core.GameDirectories.GameDirectories.GetAllGameDirectories().Count(x => x != directory && x.reference == reference) > 0) return;

                    directory.reference = reference;
                    RefreshScrollView();

                    regenerateClass = true;

                    appliedChanges = false;
                    applyButton.SetEnabled(true);
                }));
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Copy Path"), false, () =>
            {
                EditorGUIUtility.systemCopyBuffer = directory.path;
            });

            menu.AddItem(new GUIContent("Rename"), false, () =>
            {
                UnityEditor.PopupWindow.Show(element.worldBound, new GameDirectoriesPopup("Enter Directory Name", directory.name, "Rename", (string name) =>
                {
                    if (name == directory.name) return;
                    if (string.IsNullOrWhiteSpace(name)) return;
                    if (name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return;
                    if (Core.GameDirectories.GameDirectories.GetAllGameDirectories().Count(x => x != directory && x.path == directory.path[..(directory.path.LastIndexOf("/") + 1)] + name) > 0) return;

                    directory.name = name;
                    directory.path = directory.path[..(directory.path.LastIndexOf("/") + 1)] + name;
                    RefreshScrollView();

                    if (directory.reference != null) regenerateClass = true;

                    appliedChanges = false;
                    applyButton.SetEnabled(true);
                }));
            });

            menu.AddItem(new GUIContent("Move"), false, () =>
            {
                UnityEditor.PopupWindow.Show(element.worldBound, new GameDirectoriesPopup("Enter Parent Directory Path", directory.path.Contains("/") ? directory.path[..directory.path.LastIndexOf("/")] : "~", "Move", (string path) =>
                {
                    if (directory.path.LastIndexOf("/") != -1 && path == directory.path[..directory.path.LastIndexOf("/")]) return;
                    MoveGameDirectory(directory, path);
                    RefreshScrollView();
                }));
            });

            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                if (!EditorUtility.DisplayDialog("Delete Directory", $"Are you sure you want to delete the directory \"{directory.name}\"?", "Yes", "No")) return;
                RemoveGameDirectory(directory);
                RefreshScrollView();
            });

            menu.ShowAsContext();
        }

        private void RefreshScrollView()
        {
            scrollView.Clear();

            foreach (GameDirectory gameDirectory in Core.GameDirectories.GameDirectories.gameDirectories)
            {
                VisualElement subDirectoriesElement = new VisualElement();

                void CreateSubDirectories(GameDirectory directory, VisualElement parent)
                {
                    foreach (GameDirectory subDirectory in directory.subDirectories)
                    {
                        VisualElement subDirectoryElement = new VisualElement();
                        subDirectoryElement.style.flexDirection = FlexDirection.Row;
                        subDirectoryElement.style.alignItems = Align.FlexStart;

                        Image subDirectoryIcon = new Image();
                        subDirectoryIcon.image = EditorGUIUtility.IconContent("d_Project").image;
                        subDirectoryElement.Add(subDirectoryIcon);

                        if (subDirectory.subDirectories.Count == 0)
                        {
                            Label subDirectoryLabel = new Label();
                            subDirectoryLabel.text = subDirectory.name;
                            subDirectoryLabel.style.paddingLeft = 5;

                            subDirectoryLabel.RegisterCallback<MouseDownEvent>(e =>
                            {
                                if (e.button == 1)
                                {
                                    ShowDirectoryContextMenu(subDirectory, subDirectoryIcon);
                                    e.StopPropagation();
                                }
                            });

                            subDirectoryElement.Add(subDirectoryLabel);

                            parent.Add(subDirectoryElement);

                            continue;
                        }

                        Foldout subDirectoryFoldout = new Foldout();
                        subDirectoryFoldout.text = subDirectory.name;
                        subDirectoryFoldout.style.paddingLeft = 16;

                        subDirectoryFoldout.RegisterCallback<MouseDownEvent>(e =>
                        {
                            if (e.button == 1)
                            {
                                ShowDirectoryContextMenu(subDirectory, subDirectoryIcon);
                                e.StopPropagation();
                            }
                        });

                        subDirectoryElement.Add(subDirectoryFoldout);

                        CreateSubDirectories(subDirectory, subDirectoryFoldout);

                        parent.Add(subDirectoryElement);
                    }
                }

                VisualElement gameDirectoryElement = new VisualElement();
                gameDirectoryElement.style.flexDirection = FlexDirection.Row;
                gameDirectoryElement.style.alignItems = Align.FlexStart;

                Image gameDirectoryIcon = new Image();
                gameDirectoryIcon.image = EditorGUIUtility.IconContent("d_Project").image;
                gameDirectoryElement.Add(gameDirectoryIcon);

                if (gameDirectory.subDirectories.Count >= 1)
                {
                    Foldout gameDirectoryFoldout = new Foldout();
                    gameDirectoryFoldout.text = gameDirectory.name;

                    gameDirectoryFoldout.RegisterCallback<MouseDownEvent>(e =>
                    {
                        if (e.button == 1)
                        {
                            ShowDirectoryContextMenu(gameDirectory, gameDirectoryIcon);
                            e.StopPropagation();
                        }
                    });

                    gameDirectoryFoldout.Add(subDirectoriesElement);
                    gameDirectoryElement.Add(gameDirectoryFoldout);

                    CreateSubDirectories(gameDirectory, gameDirectoryFoldout);
                }
                else
                {
                    Label gameDirectoryLabel = new Label();
                    gameDirectoryLabel.text = gameDirectory.name;
                    gameDirectoryLabel.style.paddingLeft = 5;

                    gameDirectoryLabel.RegisterCallback<MouseDownEvent>(e =>
                    {
                        if (e.button == 1)
                        {
                            ShowDirectoryContextMenu(gameDirectory, gameDirectoryIcon);
                            e.StopPropagation();
                        }
                    });

                    gameDirectoryElement.Add(gameDirectoryLabel);
                }

                scrollView.Add(gameDirectoryElement);
            }
        }

        private bool ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (path.Replace("\\", "/").Contains("/"))
            {
                string[] directories = path.Replace("\\", "/").Split('/');
                if (directories.Length == 0) return false;

                foreach (string directory in directories)
                {
                    if (string.IsNullOrWhiteSpace(directory)) return false;
                    if (directory.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return false;
                }
            }
            else if (path.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return false;

            return true;
        }

        private void Apply()
        {
            appliedChanges = true;
            applyButton.SetEnabled(false);

            GameDirectory[] gameDirectories = Core.GameDirectories.GameDirectories.GetAllGameDirectories();
            GameDirectoryData[] gameDirectoryData = new GameDirectoryData[gameDirectories.Length];

            for (int i = 0; i < gameDirectories.Length; i++)
            {
                gameDirectoryData[i] = new GameDirectoryData(gameDirectories[i].path, gameDirectories[i].reference);
            }

            GameDirectoriesSettings.SetGameDirectoriesData(gameDirectoryData);

            GameDirectoriesSettings.SaveData();

            if (settingsEditor != null) settingsEditor.Refresh();

            if (regenerateClass)
            {
                GameDirectoriesSettings.GenerateClass(gameDirectories);
                regenerateClass = false;
            }
        }

        private void ShowSettingsWindow()
        {
            if (settingsEditor != null)
            {
                settingsEditor.Focus();
                return;
            }

            settingsEditor = GameDirectoriesSettingsEditor.Open();
        }
    }
}