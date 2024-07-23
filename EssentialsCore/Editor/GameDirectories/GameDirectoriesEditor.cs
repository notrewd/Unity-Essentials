using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Essentials.Inspector.Utilities;
using System.Collections.Generic;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectoriesEditor : EditorWindow
    {
        public static GameDirectoriesEditor Instance { get; private set; }

        private static Texture _folderTexture;

        private List<GameDirectory> _gameDirectories = new List<GameDirectory>();

        public bool appliedChanges = true;

        private bool _regenerateClass = false;

        private ScrollView _scrollView;
        private VisualElement _topBar;
        private VisualElement _bottomBar;
        private Button _newDirectoryButton;
        private TextField _newDirectoryField;
        private Button _applyButton;
        private Button _settingsButton;

        private GameDirectoriesSettingsEditor _settingsEditor;

        private static void SetTextures()
        {
            _folderTexture = EditorGUIUtility.isProSkin ? EditorGUIUtility.IconContent("d_Project").image : EditorGUIUtility.IconContent("Project").image;
        }

        [MenuItem("Essentials/Game Directories")]
        private static void ShowWindow()
        {
            SetTextures();

            EditorWindow window = GetWindow<GameDirectoriesEditor>();

            window.titleContent = new GUIContent("Game Directories", _folderTexture);
            window.minSize = new Vector2(300, 300);

            Instance = window as GameDirectoriesEditor;
        }

        public void CreateGUI()
        {
            GameDirectoriesSettings.LoadData();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameDirectories/GameDirectoriesEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _scrollView = rootVisualElement.Q<ScrollView>("ScrollView");
            _topBar = rootVisualElement.Q<VisualElement>("TopBar");
            _bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            _newDirectoryButton = _topBar.Q<Button>("NewDirectoryButton");
            _newDirectoryField = _topBar.Q<TextField>("NewDirectoryField");
            _applyButton = _bottomBar.Q<Button>("ApplyButton");
            _settingsButton = _bottomBar.Q<Button>("SettingsButton");

            appliedChanges = true;
            _applyButton.SetEnabled(false);
            _newDirectoryButton.SetEnabled(false);

            _newDirectoryField.RegisterValueChangedCallback(e => _newDirectoryButton.SetEnabled(ValidatePath(e.newValue)));

            _newDirectoryField.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Return && ValidatePath(_newDirectoryField.value))
                {
                    CreateGameDirectory(_newDirectoryField.value);
                    RefreshScrollView();
                    _newDirectoryField.value = string.Empty;
                }
            });

            _newDirectoryButton.clicked += () =>
            {
                if (!ValidatePath(_newDirectoryField.value)) return;

                CreateGameDirectory(_newDirectoryField.value);
                RefreshScrollView();
                _newDirectoryField.value = string.Empty;
            };

            _applyButton.clicked += Apply;
            _settingsButton.clicked += ShowSettingsWindow;

            GameDirectoryData[] gameDirectoriesData = GameDirectoriesSettings.GetGameDirectoriesData();

            if (gameDirectoriesData != null)
            {
                foreach (GameDirectoryData gameDirectoryData in gameDirectoriesData)
                {
                    CreateGameDirectory(gameDirectoryData.path, gameDirectoryData.reference);
                }

                RefreshScrollView();
            }

            _applyButton.SetEnabled(false);
            appliedChanges = true;
        }

        public GameDirectory FindGameDirectory(string path)
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

            GameDirectory gameDirectory = _gameDirectories.Find(x => x.name == directories[0]);

            if (gameDirectory == null) return null;

            GameDirectory foundDirectory = FindSubDirectory(gameDirectory, directories[1..]);

            if (foundDirectory == null) return null;

            return foundDirectory;
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

            foreach (GameDirectory gameDirectory in _gameDirectories)
            {
                directories.Add(gameDirectory);
                AddSubDirectories(gameDirectory);
            }

            return directories.ToArray();
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

            GameDirectory gameDirectory = _gameDirectories.Find(x => x.name == directories[0]);

            if (gameDirectory == null)
            {
                gameDirectory = new GameDirectory() { name = directories[0], path = directories[0] };
                _gameDirectories.Add(gameDirectory);
            }

            CreateSubDirectores(gameDirectory, directories[1..]);

            appliedChanges = false;
            _applyButton.SetEnabled(true);
        }

        private void RemoveGameDirectory(GameDirectory directory)
        {
            GameDirectory parentDirectory = FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);

            if (directory.reference != null) _regenerateClass = true;

            if (parentDirectory != null) parentDirectory.subDirectories.Remove(directory);
            else _gameDirectories.Remove(directory);

            appliedChanges = false;
            _applyButton.SetEnabled(true);
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

            if (directory.reference != null) _regenerateClass = true;

            if (newParentPath == "~")
            {
                GameDirectory oldParentDirectory = FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);

                if (oldParentDirectory != null)
                {
                    oldParentDirectory.subDirectories.Remove(directory);
                    _gameDirectories.Add(directory);
                    directory.path = directory.name;
                }

                RefreshScrollView();

                appliedChanges = false;
                _applyButton.SetEnabled(true);
                return;
            }

            GameDirectory parentDirectory = FindGameDirectory(newParentPath);

            if (parentDirectory != null)
            {
                parentDirectory.subDirectories.Add(directory);
                GameDirectory oldParentDirectory = FindGameDirectory(directory.path[..(directory.path.Contains("/") ? directory.path.LastIndexOf("/") : 0)]);
                if (oldParentDirectory != null) oldParentDirectory.subDirectories.Remove(directory);
                else _gameDirectories.Remove(directory);
                directory.path = newParentPath + "/" + directory.name;
            }

            appliedChanges = false;
            _applyButton.SetEnabled(true);
        }

        // show context menu when right clicking on a directory
        private void ShowDirectoryContextMenu(GameDirectory directory, VisualElement element)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddDisabledItem(new GUIContent($"Directory: {directory.name}"));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("New Sub Directory"), false, () =>
            {
                _newDirectoryField.value = directory.path + "/";
                _newDirectoryField.Focus();
                _newDirectoryField.SelectRange(directory.path.Length + 1, directory.path.Length + 1);
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Change Reference"), false, () =>
            {
                UnityEditor.PopupWindow.Show(element.worldBound, new GameDirectoriesPopup("Enter Reference Name", directory.reference, "Change", (string reference) =>
                {
                    if (reference == directory.reference) return;
                    if (!string.IsNullOrEmpty(reference) && !char.IsLetter(reference[0])) return;
                    foreach (char character in reference) if (!char.IsLetterOrDigit(character) && character != '_') return;
                    if (GetAllGameDirectories().Count(x => x != directory && x.reference == reference) > 0) return;

                    directory.reference = reference;
                    RefreshScrollView();

                    _regenerateClass = true;

                    appliedChanges = false;
                    _applyButton.SetEnabled(true);
                }));
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Copy Path"), false, () =>
            {
                EditorGUIUtility.systemCopyBuffer = directory.path;
            });

            menu.AddItem(new GUIContent("Rename"), false, () =>
            {
                InputPrompt.ShowWindow(this, "Rename Directory", $"Enter New Name for '{directory.name}'", directory.name, "Rename", (string name) =>
                {
                    if (name == directory.name) return;
                    if (string.IsNullOrWhiteSpace(name)) return;
                    if (name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return;
                    if (GetAllGameDirectories().Count(x => x != directory && x.path == directory.path[..(directory.path.LastIndexOf("/") + 1)] + name) > 0) return;

                    directory.name = name;
                    directory.path = directory.path[..(directory.path.LastIndexOf("/") + 1)] + name;
                    RefreshScrollView();

                    if (directory.reference != null) _regenerateClass = true;

                    appliedChanges = false;
                    _applyButton.SetEnabled(true);
                });
            });

            menu.AddItem(new GUIContent("Move"), false, () =>
            {
                InputPrompt.ShowWindow(this, "Move Directory", $"Enter Parent Directory Path for '{directory.name}'", directory.path.Contains("/") ? directory.path[..directory.path.LastIndexOf("/")] : "~", "Move", (string path) =>
                {
                    if (directory.path.LastIndexOf("/") != -1 && path == directory.path[..directory.path.LastIndexOf("/")]) return;
                    MoveGameDirectory(directory, path);
                    RefreshScrollView();
                });
            });

            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                if (!EditorUtility.DisplayDialog("Delete Directory", $"Are you sure you want to delete the directory '{directory.name}'?", "Yes", "No")) return;
                RemoveGameDirectory(directory);
                RefreshScrollView();
            });

            menu.ShowAsContext();
        }

        private void RefreshScrollView()
        {
            _scrollView.Clear();

            foreach (GameDirectory gameDirectory in _gameDirectories)
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
                        subDirectoryIcon.image = _folderTexture;
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
                gameDirectoryIcon.image = _folderTexture;
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

                _scrollView.Add(gameDirectoryElement);
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
            _applyButton.SetEnabled(false);

            GameDirectory[] gameDirectories = GetAllGameDirectories();
            GameDirectoryData[] gameDirectoryData = new GameDirectoryData[gameDirectories.Length];

            for (int i = 0; i < gameDirectories.Length; i++)
            {
                gameDirectoryData[i] = new GameDirectoryData(gameDirectories[i].path, gameDirectories[i].reference);
            }

            GameDirectoriesSettings.SetGameDirectoriesData(gameDirectoryData);

            GameDirectoriesSettings.SaveData();

            if (_settingsEditor != null) _settingsEditor.Refresh();

            if (_regenerateClass)
            {
                GameDirectoriesSettings.GenerateClass(gameDirectories);
                _regenerateClass = false;
            }
        }

        private void ShowSettingsWindow()
        {
            if (_settingsEditor != null)
            {
                _settingsEditor.Focus();
                return;
            }

            _settingsEditor = GameDirectoriesSettingsEditor.Open();
        }

        private void OnDestroy()
        {
            InputPrompt.CleanUp(this);
            if (_settingsEditor != null) _settingsEditor.Close();
        }
    }
}