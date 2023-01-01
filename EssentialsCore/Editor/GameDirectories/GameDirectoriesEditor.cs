using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Essentials.Core.GameDirectories;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectoriesEditor : EditorWindow
    {
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
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameDirectories/GameDirectoriesEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            scrollView = rootVisualElement.Q<ScrollView>("ScrollView");
            topBar = rootVisualElement.Q<VisualElement>("TopBar");
            bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            newDirectoryButton = topBar.Q<Button>("NewDirectoryButton");
            newDirectoryField = topBar.Q<TextField>("NewDirectoryField");
            applyButton = bottomBar.Q<Button>("ApplyButton");
            settingsButton = bottomBar.Q<Button>("SettingsButton");

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
        }

        private void CreateGameDirectory(string path)
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
                if (subDirectories.Length == 0) return;

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

            applyButton.SetEnabled(true);
        }

        private GameDirectory FindGameDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            string[] directories = Array.Empty<string>();

            if (path.Contains("\\")) path = path.Replace("\\", "/");
            if (path.Contains("/")) directories = path.Split('/');
            else directories = new string[] { path };

            if (directories.Length == 0) return null;

            foreach (string directory in directories)
            {
                if (string.IsNullOrWhiteSpace(directory)) return null;
                if (directory.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return null;
            }

            GameDirectory FindSubDirectory(GameDirectory directory, string[] subDirectories)
            {
                if (subDirectories.Length == 0) return directory;

                GameDirectory subDirectory = directory.subDirectories.Find(x => x.name == subDirectories[0]);

                if (subDirectory == null) return null;

                return FindSubDirectory(subDirectory, subDirectories[1..]);
            }

            GameDirectory gameDirectory = Core.GameDirectories.GameDirectories.gameDirectories.Find(x => x.name == directories[0]);

            if (gameDirectory == null) return null;

            GameDirectory foundDirectory = FindSubDirectory(gameDirectory, directories[1..]);

            if (foundDirectory == null) return null;

            return foundDirectory;
        }

        private void RemoveGameDirectory(GameDirectory directory)
        {
            GameDirectory parentDirectory = FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);

            if (parentDirectory != null) parentDirectory.subDirectories.Remove(directory);
            else Core.GameDirectories.GameDirectories.gameDirectories.Remove(directory);

            applyButton.SetEnabled(true);
        }

        private void MoveGameDirectory(GameDirectory directory, string newParentPath)
        {
            if (string.IsNullOrWhiteSpace(newParentPath)) return;
            if (newParentPath.Contains("\\")) newParentPath = newParentPath.Replace("\\", "/");

            if (newParentPath[^1] == '/') newParentPath = newParentPath[..^1];

            if (newParentPath == directory.path) return;

            if (newParentPath.Length == 1 && newParentPath == ".") newParentPath = "~";
            else if (newParentPath.Length == 2 && (newParentPath == "~/" || newParentPath == "./")) newParentPath = "~";
            else if (newParentPath.Length > 2 && (newParentPath[..2] == "./" || newParentPath[..2] == "~/")) newParentPath = newParentPath[2..];

            if (newParentPath == "~")
            {
                GameDirectory oldParentDirectory = FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);

                if (oldParentDirectory != null)
                {
                    oldParentDirectory.subDirectories.Remove(directory);
                    Core.GameDirectories.GameDirectories.gameDirectories.Add(directory);
                    directory.path = directory.name;
                }

                RefreshScrollView();
                return;
            }

            GameDirectory parentDirectory = FindGameDirectory(newParentPath);

            if (parentDirectory != null)
            {
                parentDirectory.subDirectories.Add(directory);
                GameDirectory oldParentDirectory = FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);
                if (oldParentDirectory != null) oldParentDirectory.subDirectories.Remove(directory);
                else Core.GameDirectories.GameDirectories.gameDirectories.Remove(directory);
                directory.path = newParentPath + "/" + directory.name;
            }

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

            menu.AddItem(new GUIContent("Copy Path"), false, () =>
            {
                EditorGUIUtility.systemCopyBuffer = directory.path;
            });

            menu.AddItem(new GUIContent("Rename"), false, () =>
            {
                UnityEditor.PopupWindow.Show(element.worldBound, new GameDirectoriesPopup("Enter Directory Name", directory.name, "Rename", (string name) =>
                {
                    if (string.IsNullOrWhiteSpace(name)) return;
                    if (name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return;

                    directory.name = name;
                    directory.path = directory.path[..(directory.path.LastIndexOf("/") + 1)] + name;
                    RefreshScrollView();
                }));
            });

            menu.AddItem(new GUIContent("Move"), false, () =>
            {
                UnityEditor.PopupWindow.Show(element.worldBound, new GameDirectoriesPopup("Enter Directory Path", directory.path.Contains("/") ? directory.path[..directory.path.LastIndexOf("/")] : "~", "Move", (string path) =>
                {
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

        public GameDirectory[] GetAllGameDirectories()
        {
            List<GameDirectory> directories = new List<GameDirectory>();

            void AddSubDirectories(GameDirectory directory)
            {
                foreach (GameDirectory subDirectory in directory.subDirectories)
                {
                    directories.Add(subDirectory);
                    AddSubDirectories(subDirectory);
                }
            }

            foreach (GameDirectory gameDirectory in Core.GameDirectories.GameDirectories.gameDirectories)
            {
                directories.Add(gameDirectory);
                AddSubDirectories(gameDirectory);
            }

            return directories.ToArray();
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
                string[] directories = path.Split('/');
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
            applyButton.SetEnabled(false);
            if (settingsEditor != null) settingsEditor.Refresh();
        }

        private void ShowSettingsWindow()
        {
            if (settingsEditor != null)
            {
                settingsEditor.Focus();
                return;
            }

            settingsEditor = EditorWindow.GetWindow<GameDirectoriesSettingsEditor>();
            settingsEditor.titleContent = new GUIContent("Game Directories Settings", EditorGUIUtility.IconContent("d_SettingsIcon").image);
            settingsEditor.minSize = new Vector2(300, 300);
        }
    }
}