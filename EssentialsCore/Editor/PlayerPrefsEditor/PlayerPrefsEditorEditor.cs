using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.Serialization.Plists;
using UnityEditor.UIElements;
using System.Linq;

namespace Essentials.Internal.PlayerPrefsEditor
{
    public class PlayerPrefsEditorEditor : EditorWindow
    {
        public static bool isEditorPrefs { get; private set; } = false;
        private static bool shownEditorPrefsWarning = false;

        private VisualElement topBar;
        private Button playerPrefsButton;
        private Button editorPrefsButton;
        private ToolbarSearchField searchField;
        private Button orderButton;
        private Button refreshButton;
        private ScrollView playerPrefsList;
        private VisualElement noPlayerPrefsInfo;
        private Button infoRefreshButton;
        private VisualElement bottomBar;
        private Label internalPlayerPrefsLabel;
        private Toggle internalPlayerPrefsToggle;
        private Button newPlayerPrefButton;
        private Button applyButton;

        private Dictionary<string, object> playerPrefs = new Dictionary<string, object>();
        private bool orderAscending = true;

#if PLATFORM_STANDALONE_OSX || PLATFORM_STANDALONE_WIN
        [MenuItem("Essentials/PlayerPrefs Editor")]
        private static void ShowWindow()
        {
            PlayerPrefsEditorEditor window = GetWindow<PlayerPrefsEditorEditor>();
            window.titleContent = new GUIContent("PlayerPrefs Editor");
            window.minSize = new Vector2(350, 300);
        }
#endif

        public void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/PlayerPrefsEditor/PlayerPrefsEditorEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            isEditorPrefs = false;

            topBar = rootVisualElement.Q<VisualElement>("TopBar");
            playerPrefsButton = topBar.Q<Button>("PlayerPrefsButton");
            editorPrefsButton = topBar.Q<Button>("EditorPrefsButton");
            searchField = topBar.Q<ToolbarSearchField>("SearchField");
            orderButton = topBar.Q<Button>("OrderButton");
            refreshButton = topBar.Q<Button>("RefreshButton");
            playerPrefsList = rootVisualElement.Q<ScrollView>("PlayerPrefsList");
            noPlayerPrefsInfo = rootVisualElement.Q<VisualElement>("NoPlayerPrefsInfo");
            infoRefreshButton = noPlayerPrefsInfo.Q<Button>("RefreshButton");
            bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            internalPlayerPrefsLabel = bottomBar.Q<Label>("InternalPlayerPrefsLabel");
            internalPlayerPrefsToggle = bottomBar.Q<Toggle>("InternalPlayerPrefsToggle");
            newPlayerPrefButton = bottomBar.Q<Button>("NewPlayerPrefButton");
            applyButton = bottomBar.Q<Button>("ApplyButton");

            playerPrefsButton.clicked += () =>
            {
                if (!isEditorPrefs) return;
                if (applyButton.enabledSelf) if (!EditorUtility.DisplayDialog("Warning", "You have unsaved changes. Are you sure you want to continue?", "Yes", "No")) return;

                isEditorPrefs = false;

                playerPrefsButton.AddToClassList("selected");
                editorPrefsButton.RemoveFromClassList("selected");

                internalPlayerPrefsLabel.style.display = DisplayStyle.Flex;
                internalPlayerPrefsToggle.style.display = DisplayStyle.Flex;

                newPlayerPrefButton.text = "New PlayerPref";

                applyButton.SetEnabled(false);

                RefreshAll();
            };

            editorPrefsButton.clicked += () =>
            {
                if (isEditorPrefs) return;

                if (!shownEditorPrefsWarning && !EditorUtility.DisplayDialog("Warning", "You are about to edit the EditorPrefs. This may cause the editor to crash or behave unexpectedly. Are you sure you want to continue?", "Yes", "No")) return;
                shownEditorPrefsWarning = true;

                if (applyButton.enabledSelf) if (!EditorUtility.DisplayDialog("Warning", "You have unsaved changes. Are you sure you want to continue?", "Yes", "No")) return;

                isEditorPrefs = true;

                playerPrefsButton.RemoveFromClassList("selected");
                editorPrefsButton.AddToClassList("selected");

                internalPlayerPrefsLabel.style.display = DisplayStyle.None;
                internalPlayerPrefsToggle.style.display = DisplayStyle.None;

                newPlayerPrefButton.text = "New EditorPref";

                applyButton.SetEnabled(false);

                RefreshAll();
            };

            searchField.RegisterValueChangedCallback((_) => RefreshList());

            orderButton.clicked += () =>
            {
                orderAscending = !orderAscending;

                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.notrewd.essentials/EssentialsCore/Icons/" + (orderAscending ? "ascending_icon" : "descending_icon") + ".png");
                orderButton.style.backgroundImage = new StyleBackground(icon);

                RefreshList();
            };

            refreshButton.clicked += RefreshAll;

            infoRefreshButton.clicked += RefreshAll;

            internalPlayerPrefsToggle.RegisterValueChangedCallback((_) => RefreshList());

            newPlayerPrefButton.clicked += () => PlayerPrefsAddEditor.ShowWindow(this);
            applyButton.clicked += Apply;

            RefreshAll();
        }

        public void AddPlayerPref(string key, object value)
        {
            if (playerPrefs.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("Error", "Key already exists", "OK");
                return;
            }

            playerPrefs.Add(key, value);

            RefreshList();
            applyButton.SetEnabled(true);
        }

        private void RefreshAll()
        {
            LoadPlayerPrefs();
            RefreshList();

            applyButton.SetEnabled(false);
        }

        private void LoadPlayerPrefs()
        {
#if PLATFORM_STANDALONE_OSX
            BinaryPlistReader reader = new BinaryPlistReader();

            if (!isEditorPrefs)
            {
                IDictionary values = reader.ReadObject("/Users/" + Environment.UserName + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist");

                if (values != null)
                {
                    playerPrefs.Clear();

                    foreach (string key in values.Keys)
                    {
                        if (!PlayerPrefs.HasKey(key)) continue;

                        if (PlayerPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") playerPrefs.Add(key, PlayerPrefs.GetString(key));
                        else if (PlayerPrefs.GetInt(key, int.MinValue) != int.MinValue) playerPrefs.Add(key, PlayerPrefs.GetInt(key));
                        else if (PlayerPrefs.GetFloat(key, float.NaN) != float.NaN) playerPrefs.Add(key, PlayerPrefs.GetFloat(key));
                    }
                }
            }
            else
            {
                IDictionary values = reader.ReadObject("/Users/" + Environment.UserName + "/Library/Preferences/com.unity3d.UnityEditor5.x.plist");

                if (values != null)
                {
                    playerPrefs.Clear();

                    foreach (string key in values.Keys)
                    {
                        if (!EditorPrefs.HasKey(key)) continue;

                        if (EditorPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") playerPrefs.Add(key, EditorPrefs.GetString(key));
                        else if (EditorPrefs.GetInt(key, int.MinValue) != int.MinValue) playerPrefs.Add(key, EditorPrefs.GetInt(key));
                        else if (EditorPrefs.GetFloat(key, float.NaN) != float.NaN) playerPrefs.Add(key, EditorPrefs.GetFloat(key));
                    }
                }
            }
#endif

#if PLATFORM_STANDALONE_WIN
            playerPrefs.Clear();

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

                        if (PlayerPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") playerPrefs.Add(key, PlayerPrefs.GetString(key));
                        else if (PlayerPrefs.GetInt(key, int.MinValue) != int.MinValue) playerPrefs.Add(key, PlayerPrefs.GetInt(key));
                        else if (PlayerPrefs.GetFloat(key, float.NaN) != float.NaN) playerPrefs.Add(key, PlayerPrefs.GetFloat(key));
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

                        if (EditorPrefs.GetString(key, "Essentials.PlayerPrefsEditor.NoKey") != "Essentials.PlayerPrefsEditor.NoKey") playerPrefs.Add(key, EditorPrefs.GetString(key));
                        else if (EditorPrefs.GetInt(key, int.MinValue) != int.MinValue) playerPrefs.Add(key, EditorPrefs.GetInt(key));
                        else if (EditorPrefs.GetFloat(key, float.NaN) != float.NaN) playerPrefs.Add(key, EditorPrefs.GetFloat(key));
                    }
                }
            }
#endif
        }

        private void RefreshList()
        {
            playerPrefsList.Clear();

            if (orderAscending) playerPrefs = playerPrefs.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            else playerPrefs = playerPrefs.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            foreach (KeyValuePair<string, object> pair in playerPrefs)
            {
                if (!internalPlayerPrefsToggle.value && (pair.Key.ToString().StartsWith("unity.") || pair.Key.ToString() == "UnityGraphicsQuality")) continue;
                if (!pair.Key.ToString().ToLower().Contains(searchField.value.ToLower())) continue;

                VisualElement element = new VisualElement();
                element.style.flexDirection = FlexDirection.Row;
                element.style.marginBottom = 2;

                TextField valueField = new TextField();
                valueField.label = pair.Key.ToString();
                valueField.value = pair.Value.ToString();
                valueField.style.flexShrink = 1;
                valueField.style.flexGrow = 1;

                DropdownField typeField = new DropdownField();
                typeField.style.width = 70;
                typeField.choices = new List<string>() { "String", "Int", "Float" };

                if (pair.Value is string) typeField.value = "String";
                else if (pair.Value is int) typeField.value = "Int";
                else if (pair.Value is float) typeField.value = "Float";

                valueField.RegisterValueChangedCallback((evt) =>
                {
                    applyButton.SetEnabled(true);

                    if (typeField.value == "Int")
                    {
                        if (!int.TryParse(evt.newValue, out int value)) applyButton.SetEnabled(false);
                    }
                    else if (typeField.value == "Float")
                    {
                        if (!float.TryParse(evt.newValue, out float value)) applyButton.SetEnabled(false);
                    }
                });

                valueField.RegisterCallback<FocusOutEvent>((_) =>
                {
                    if (typeField.value == "Int")
                    {
                        if (!int.TryParse(valueField.value, out int value)) return;
                        playerPrefs[pair.Key] = int.Parse(valueField.value);
                    }
                    else if (typeField.value == "Float")
                    {
                        if (!float.TryParse(valueField.value, out float value)) return;
                        playerPrefs[pair.Key] = float.Parse(valueField.value);
                    }
                    else playerPrefs[pair.Key] = valueField.value;
                });

                typeField.RegisterValueChangedCallback((evt) =>
                {
                    if (evt.newValue == "String") playerPrefs[pair.Key] = valueField.value;
                    else if (evt.newValue == "Int")
                    {
                        if (!int.TryParse(valueField.value, out int value)) valueField.value = "0";
                        playerPrefs[pair.Key] = int.Parse(valueField.value);
                    }
                    else if (evt.newValue == "Float")
                    {
                        if (!float.TryParse(valueField.value, out float value)) valueField.value = "0";
                        playerPrefs[pair.Key] = float.Parse(valueField.value);
                    }

                    applyButton.SetEnabled(true);
                });

                ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator((evt) =>
                {
                    evt.menu.AppendAction(isEditorPrefs ? "Delete EditorPref" : "Delete PlayerPref", (_) =>
                    {
                        playerPrefs.Remove(pair.Key);
                        RefreshList();
                        applyButton.SetEnabled(true);
                    });
                });

                element.AddManipulator(menuManipulator);

                element.Add(valueField);
                element.Add(typeField);
                playerPrefsList.Add(element);
            }

            if (playerPrefsList.childCount > 0)
            {
                noPlayerPrefsInfo.style.display = DisplayStyle.None;
                playerPrefsList.style.display = DisplayStyle.Flex;
            }
            else
            {
                noPlayerPrefsInfo.style.display = DisplayStyle.Flex;
                playerPrefsList.style.display = DisplayStyle.None;
            }
        }

        private void Apply()
        {
            if (!isEditorPrefs)
            {
                PlayerPrefs.DeleteAll();

                foreach (KeyValuePair<string, object> pair in playerPrefs)
                {
                    if (pair.Value is string) PlayerPrefs.SetString(pair.Key.ToString(), pair.Value.ToString());
                    else if (pair.Value is int) PlayerPrefs.SetInt(pair.Key.ToString(), int.Parse(pair.Value.ToString()));
                    else if (pair.Value is float or double) PlayerPrefs.SetFloat(pair.Key.ToString(), float.Parse(pair.Value.ToString()));
                }

                PlayerPrefs.Save();

                applyButton.SetEnabled(false);
            }
            else
            {
                EditorPrefs.DeleteAll();

                foreach (KeyValuePair<string, object> pair in playerPrefs)
                {
                    if (pair.Value is string) EditorPrefs.SetString(pair.Key.ToString(), pair.Value.ToString());
                    else if (pair.Value is int) EditorPrefs.SetInt(pair.Key.ToString(), int.Parse(pair.Value.ToString()));
                    else if (pair.Value is float) EditorPrefs.SetFloat(pair.Key.ToString(), float.Parse(pair.Value.ToString()));
                    else if (pair.Value is bool) EditorPrefs.SetBool(pair.Key.ToString(), bool.Parse(pair.Value.ToString()));
                }

                applyButton.SetEnabled(false);
            }
        }
    }
}