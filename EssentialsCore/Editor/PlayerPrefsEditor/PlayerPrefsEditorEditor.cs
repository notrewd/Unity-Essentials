using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.Serialization.Plists;
using UnityEditor.UIElements;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using Essentials.Inspector.Utilities;

namespace Essentials.Internal.PlayerPrefsEditor
{
    public class PlayerPrefsEditorEditor : EditorWindow
    {
        public static bool isEditorPrefs { get; private set; } = false;
        private static bool _shownEditorPrefsWarning = false;
#if UNITY_EDITOR_WIN
        private static bool _watchingChanges = true;
#endif
        private VisualElement _topBar;
        private Button _playerPrefsButton;
        private Button _editorPrefsButton;
        private ToolbarPopupSearchField _searchField;
        private Button _watchButton;
        private Button _orderButton;
        private Button _refreshButton;
        private ScrollView _playerPrefsList;
        private VisualElement _noPlayerPrefsInfo;
        private Button _infoRefreshButton;
        private VisualElement _bottomBar;
        private Label _internalPlayerPrefsLabel;
        private Toggle _internalPlayerPrefsToggle;
        private Button _newPlayerPrefButton;
        private Button _applyButton;

        private RegistryMonitor _playerPrefsRegistryMonitor;
        private RegistryMonitor _editorPrefsRegistryMonitor;
        private Dictionary<string, object> _playerPrefs = new Dictionary<string, object>();
        private SearchType _searchType = SearchType.KeyName;
        private bool _orderAscending = true;
        private bool _playerPrefsEntryUpdated = false;
        private bool _editorPrefsEntryUpdated = false;

        private enum SearchType { KeyName, Value, ValueType }

#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
        [MenuItem("Essentials/PlayerPrefs Editor")]
        private static void ShowWindow()
        {
            PlayerPrefsEditorEditor window = GetWindow<PlayerPrefsEditorEditor>();
            window.titleContent = new GUIContent("PlayerPrefs Editor", IconDatabase.GetIcon("PlayerPrefs@32"));
            window.minSize = new Vector2(350, 300);
        }
#endif

        private void SetupButtonIcons()
        {
#if UNITY_EDITOR_WIN
            _watchButton.style.backgroundImage = IconDatabase.GetIcon("Watch@32");
#endif
            _orderButton.style.backgroundImage = IconDatabase.GetIcon("Ascending@32");
            _refreshButton.style.backgroundImage = IconDatabase.GetIcon("Refresh@32");
        }

        public void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/PlayerPrefsEditor/PlayerPrefsEditorEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            isEditorPrefs = false;

            _topBar = rootVisualElement.Q<VisualElement>("TopBar");
            _playerPrefsButton = _topBar.Q<Button>("PlayerPrefsButton");
            _editorPrefsButton = _topBar.Q<Button>("EditorPrefsButton");
            _searchField = _topBar.Q<ToolbarPopupSearchField>("SearchField");
            _watchButton = _topBar.Q<Button>("WatchButton");
            _orderButton = _topBar.Q<Button>("OrderButton");
            _refreshButton = _topBar.Q<Button>("RefreshButton");
            _playerPrefsList = rootVisualElement.Q<ScrollView>("PlayerPrefsList");
            _noPlayerPrefsInfo = rootVisualElement.Q<VisualElement>("NoPlayerPrefsInfo");
            _infoRefreshButton = _noPlayerPrefsInfo.Q<Button>("RefreshButton");
            _bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            _internalPlayerPrefsLabel = _bottomBar.Q<Label>("InternalPlayerPrefsLabel");
            _internalPlayerPrefsToggle = _bottomBar.Q<Toggle>("InternalPlayerPrefsToggle");
            _newPlayerPrefButton = _bottomBar.Q<Button>("NewPlayerPrefButton");
            _applyButton = _bottomBar.Q<Button>("ApplyButton");

            SetupButtonIcons();

            _playerPrefsButton.clicked += () =>
            {
                if (!isEditorPrefs) return;
                if (_applyButton.enabledSelf) if (!EditorUtility.DisplayDialog("Warning", "You have unsaved changes. Are you sure you want to continue?", "Yes", "No")) return;

                isEditorPrefs = false;

                _playerPrefsButton.AddToClassList("selected");
                _editorPrefsButton.RemoveFromClassList("selected");

                _internalPlayerPrefsLabel.style.display = DisplayStyle.Flex;
                _internalPlayerPrefsToggle.style.display = DisplayStyle.Flex;

                _newPlayerPrefButton.text = "New PlayerPref";

                _applyButton.SetEnabled(false);

                RefreshAll();
            };

            _editorPrefsButton.clicked += () =>
            {
                if (isEditorPrefs) return;

                if (!_shownEditorPrefsWarning && !EditorUtility.DisplayDialog("Warning", "You are about to edit the EditorPrefs. This may cause the editor to crash or behave unexpectedly. Are you sure you want to continue?", "Yes", "No")) return;
                _shownEditorPrefsWarning = true;

                if (_applyButton.enabledSelf) if (!EditorUtility.DisplayDialog("Warning", "You have unsaved changes. Are you sure you want to continue?", "Yes", "No")) return;

                isEditorPrefs = true;

                _playerPrefsButton.RemoveFromClassList("selected");
                _editorPrefsButton.AddToClassList("selected");

                _internalPlayerPrefsLabel.style.display = DisplayStyle.None;
                _internalPlayerPrefsToggle.style.display = DisplayStyle.None;

                _newPlayerPrefButton.text = "New EditorPref";

                _applyButton.SetEnabled(false);

                RefreshAll();
            };

            _searchField.menu.AppendAction("Key Name", (_) =>
            {
                _searchType = SearchType.KeyName;
                RefreshList();
            }, (_) => _searchType == SearchType.KeyName ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            _searchField.menu.AppendAction("Value", (_) =>
            {
                _searchType = SearchType.Value;
                RefreshList();
            }, (_) => _searchType == SearchType.Value ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            _searchField.menu.AppendAction("Value Type", (_) =>
            {
                _searchType = SearchType.ValueType;
                RefreshList();
            }, (_) => _searchType == SearchType.ValueType ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            _searchField.RegisterValueChangedCallback((_) => RefreshList());

#if UNITY_EDITOR_WIN
            _playerPrefsRegistryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, "Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
            _playerPrefsRegistryMonitor.RegChanged += (_, __) => _playerPrefsEntryUpdated = true;
            if (_watchingChanges) _playerPrefsRegistryMonitor.Start();

            _editorPrefsRegistryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, "Software\\Unity Technologies\\Unity Editor 5.x");
            _editorPrefsRegistryMonitor.RegChanged += (_, __) => _editorPrefsEntryUpdated = true;
            if (_watchingChanges) _editorPrefsRegistryMonitor.Start();

            _watchButton.style.display = DisplayStyle.Flex;

            if (!_watchingChanges)
            {
                Texture2D icon = IconDatabase.GetIcon("Unwatch@32");
                _watchButton.style.backgroundImage = new StyleBackground(icon);

                _watchButton.tooltip = "Watch Changes";
            }

            _watchButton.clicked += () =>
            {
                _watchingChanges = !_watchingChanges;

                Texture2D icon = _watchingChanges ? IconDatabase.GetIcon("Watch@32") : IconDatabase.GetIcon("Unwatch@32");
                _watchButton.style.backgroundImage = new StyleBackground(icon);

                if (_watchingChanges)
                {
                    _playerPrefsRegistryMonitor.Start();
                    _editorPrefsRegistryMonitor.Start();

                    _watchButton.tooltip = "Unwatch Changes";
                }
                else
                {
                    _playerPrefsRegistryMonitor.Stop();
                    _editorPrefsRegistryMonitor.Stop();

                    _watchButton.tooltip = "Watch Changes";
                }
            };
#endif

            _orderButton.clicked += () =>
            {
                _orderAscending = !_orderAscending;

                Texture2D icon = _orderAscending ? IconDatabase.GetIcon("Ascending@32") : IconDatabase.GetIcon("Descending@32");
                _orderButton.style.backgroundImage = new StyleBackground(icon);

                RefreshList();
            };

            _refreshButton.clicked += RefreshAll;

            _infoRefreshButton.clicked += RefreshAll;

            _internalPlayerPrefsToggle.RegisterValueChangedCallback((_) => RefreshList());

            _newPlayerPrefButton.clicked += () => PlayerPrefsAddEditor.ShowWindow(this);

            _applyButton.clicked += Apply;

            RefreshAll();
        }

        private void OnEnable() => EditorApplication.update += Update;

        private void OnDisable() => EditorApplication.update -= Update;

        private void Update()
        {
            if (_playerPrefsEntryUpdated)
            {
                _playerPrefsEntryUpdated = false;
                if (isEditorPrefs) return;
                LoadPlayerPrefs();
                RefreshList();
            }

            if (_editorPrefsEntryUpdated)
            {
                _editorPrefsEntryUpdated = false;
                if (!isEditorPrefs) return;
                LoadPlayerPrefs();
                RefreshList();
            }
        }

        public void AddPlayerPref(string key, object value)
        {
            if (_playerPrefs.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("Error", "Key already exists", "OK");
                return;
            }

            _playerPrefs.Add(key, value);

            RefreshList();
            _applyButton.SetEnabled(true);
        }

        private void RefreshAll()
        {
            LoadPlayerPrefs();
            RefreshList();

            _applyButton.SetEnabled(false);
        }

        private void LoadPlayerPrefs()
        {
#if UNITY_EDITOR_OSX
            BinaryPlistReader reader = new BinaryPlistReader();

            if (!isEditorPrefs)
            {
                IDictionary values = reader.ReadObject("/Users/" + Environment.UserName + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist");

                if (values != null)
                {
                    _playerPrefs.Clear();

                    foreach (string key in values.Keys)
                    {
                        if (!PlayerPrefs.HasKey(key)) continue;

                        if (PlayerPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") _playerPrefs.Add(key, PlayerPrefs.GetString(key));
                        else if (PlayerPrefs.GetInt(key, int.MinValue) != int.MinValue) _playerPrefs.Add(key, PlayerPrefs.GetInt(key));
                        else if (PlayerPrefs.GetFloat(key, float.NaN) != float.NaN) _playerPrefs.Add(key, PlayerPrefs.GetFloat(key));
                    }
                }
            }
            else
            {
                IDictionary values = reader.ReadObject("/Users/" + Environment.UserName + "/Library/Preferences/com.unity3d.UnityEditor5.x.plist");

                if (values != null)
                {
                    _playerPrefs.Clear();

                    foreach (string key in values.Keys)
                    {
                        if (!EditorPrefs.HasKey(key)) continue;

                        if (EditorPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") _playerPrefs.Add(key, EditorPrefs.GetString(key));
                        else if (EditorPrefs.GetInt(key, int.MinValue) != int.MinValue) _playerPrefs.Add(key, EditorPrefs.GetInt(key));
                        else if (EditorPrefs.GetFloat(key, float.NaN) != float.NaN) _playerPrefs.Add(key, EditorPrefs.GetFloat(key));
                    }
                }
            }
#endif

#if UNITY_EDITOR_WIN
            _playerPrefs.Clear();

            if (!isEditorPrefs)
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);

                if (registryKey != null)
                {
                    foreach (string valueName in registryKey.GetValueNames())
                    {
                        string key = valueName;
                        if (key.Contains("_h")) key = key.Substring(0, key.LastIndexOf("_h"));

                        if (!PlayerPrefs.HasKey(key)) continue;

                        if (PlayerPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") _playerPrefs.Add(key, PlayerPrefs.GetString(key));
                        else if (PlayerPrefs.GetInt(key, int.MinValue) != int.MinValue) _playerPrefs.Add(key, PlayerPrefs.GetInt(key));
                        else if (PlayerPrefs.GetFloat(key, float.NaN) != float.NaN) _playerPrefs.Add(key, PlayerPrefs.GetFloat(key));
                    }
                }
            }
            else
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Unity Technologies\\Unity Editor 5.x");

                if (registryKey != null)
                {
                    foreach (string valueName in registryKey.GetValueNames())
                    {
                        string key = valueName;
                        if (key.Contains("_h")) key = key.Substring(0, key.LastIndexOf("_h"));

                        if (!EditorPrefs.HasKey(key)) continue;

                        if (EditorPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") _playerPrefs.Add(key, EditorPrefs.GetString(key));
                        else if (EditorPrefs.GetInt(key, int.MinValue) != int.MinValue) _playerPrefs.Add(key, EditorPrefs.GetInt(key));
                        else if (EditorPrefs.GetFloat(key, float.NaN) != float.NaN) _playerPrefs.Add(key, EditorPrefs.GetFloat(key));
                    }
                }
            }
#endif
        }

        private void RefreshList()
        {
            _playerPrefsList.Clear();

            if (_orderAscending) _playerPrefs = _playerPrefs.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            else _playerPrefs = _playerPrefs.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            int index = 0;

            foreach (KeyValuePair<string, object> pair in _playerPrefs)
            {
                if (!_internalPlayerPrefsToggle.value && (pair.Key.ToString().StartsWith("unity.") || pair.Key.ToString() == "UnityGraphicsQuality")) continue;

                if (_searchType == SearchType.KeyName && !pair.Key.ToString().ToLower().Contains(_searchField.value.ToLower())) continue;
                else if (_searchType == SearchType.Value && pair.Value.ToString().ToLower() != _searchField.value.ToLower()) continue;
                else if (_searchType == SearchType.ValueType)
                {
                    switch (pair.Value)
                    {
                        case string _:
                            if (_searchField.value.ToLower() != "string") continue;
                            break;
                        case int _:
                            if (_searchField.value.ToLower() != "int") continue;
                            break;
                        case float _:
                            if (_searchField.value.ToLower() != "float") continue;
                            break;
                        default:
                            continue;
                    }
                }

                VisualElement element = new VisualElement();
                element.style.flexDirection = FlexDirection.Row;
                element.style.marginBottom = 2;

                if (index % 2 == 1) element.AddToClassList("secondary");

                TextField valueField = new TextField();
                valueField.label = pair.Key.ToString();
                valueField.value = pair.Value.ToString();
                valueField.style.flexShrink = 1;
                valueField.style.flexGrow = 1;

                DropdownField typeField = new DropdownField();
                typeField.style.width = 70;
                typeField.style.height = 20;
                typeField.choices = new List<string>() { "String", "Int", "Float" };

                if (pair.Value is string) typeField.value = "String";
                else if (pair.Value is int) typeField.value = "Int";
                else if (pair.Value is float) typeField.value = "Float";

                valueField.RegisterValueChangedCallback((evt) =>
                {
                    _applyButton.SetEnabled(true);

                    if (typeField.value == "Int")
                    {
                        if (!int.TryParse(evt.newValue, out int value)) _applyButton.SetEnabled(false);
                    }
                    else if (typeField.value == "Float")
                    {
                        if (!float.TryParse(evt.newValue, out float value)) _applyButton.SetEnabled(false);
                    }
                });

                valueField.RegisterCallback<FocusOutEvent>((_) =>
                {
                    if (typeField.value == "Int")
                    {
                        if (!int.TryParse(valueField.value, out int value)) return;
                        _playerPrefs[pair.Key] = int.Parse(valueField.value);
                    }
                    else if (typeField.value == "Float")
                    {
                        if (!float.TryParse(valueField.value, out float value)) return;
                        _playerPrefs[pair.Key] = float.Parse(valueField.value);
                    }
                    else _playerPrefs[pair.Key] = valueField.value;
                });

                typeField.RegisterValueChangedCallback((evt) =>
                {
                    if (evt.newValue == "String") _playerPrefs[pair.Key] = valueField.value;
                    else if (evt.newValue == "Int")
                    {
                        if (!int.TryParse(valueField.value, out int value)) valueField.value = "0";
                        _playerPrefs[pair.Key] = int.Parse(valueField.value);
                    }
                    else if (evt.newValue == "Float")
                    {
                        if (!float.TryParse(valueField.value, out float value)) valueField.value = "0";
                        _playerPrefs[pair.Key] = float.Parse(valueField.value);
                    }

                    _applyButton.SetEnabled(true);
                });

                ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator((evt) =>
                {
                    evt.menu.AppendAction(isEditorPrefs ? "Delete EditorPref" : "Delete PlayerPref", (_) =>
                    {
                        _playerPrefs.Remove(pair.Key);
                        RefreshList();
                        _applyButton.SetEnabled(true);
                    });
                });

                element.AddManipulator(menuManipulator);

                element.Add(valueField);
                element.Add(typeField);
                _playerPrefsList.Add(element);

                index++;
            }

            if (_playerPrefsList.childCount > 0)
            {
                _noPlayerPrefsInfo.style.display = DisplayStyle.None;
                _playerPrefsList.style.display = DisplayStyle.Flex;
            }
            else
            {
                _noPlayerPrefsInfo.style.display = DisplayStyle.Flex;
                _playerPrefsList.style.display = DisplayStyle.None;
            }
        }

        private void Apply()
        {
#if UNITY_EDITOR_WIN
            if (_watchingChanges)
            {
                _playerPrefsRegistryMonitor.Stop();
                _editorPrefsRegistryMonitor.Stop();
            }
#endif

            if (!isEditorPrefs)
            {
                PlayerPrefs.DeleteAll();

                foreach (KeyValuePair<string, object> pair in _playerPrefs)
                {
                    if (pair.Value is string) PlayerPrefs.SetString(pair.Key.ToString(), pair.Value.ToString());
                    else if (pair.Value is int) PlayerPrefs.SetInt(pair.Key.ToString(), int.Parse(pair.Value.ToString()));
                    else if (pair.Value is float) PlayerPrefs.SetFloat(pair.Key.ToString(), float.Parse(pair.Value.ToString()));
                }

                PlayerPrefs.Save();

                _applyButton.SetEnabled(false);
            }
            else
            {
                EditorPrefs.DeleteAll();

                foreach (KeyValuePair<string, object> pair in _playerPrefs)
                {
                    if (pair.Value is string) EditorPrefs.SetString(pair.Key.ToString(), pair.Value.ToString());
                    else if (pair.Value is int) EditorPrefs.SetInt(pair.Key.ToString(), int.Parse(pair.Value.ToString()));
                    else if (pair.Value is float) EditorPrefs.SetFloat(pair.Key.ToString(), float.Parse(pair.Value.ToString()));
                    else if (pair.Value is bool) EditorPrefs.SetBool(pair.Key.ToString(), bool.Parse(pair.Value.ToString()));
                }

                _applyButton.SetEnabled(false);
            }

#if UNITY_EDITOR_WIN
            if (_watchingChanges)
            {
                _playerPrefsRegistryMonitor.Start();
                _editorPrefsRegistryMonitor.Start();
            }
#endif
        }
    }
}