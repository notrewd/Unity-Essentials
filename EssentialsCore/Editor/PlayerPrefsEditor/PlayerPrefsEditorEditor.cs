using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Plists;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPrefsEditorEditor : EditorWindow
{
    private ScrollView playerPrefsList;
    private VisualElement noPlayerPrefsInfo;
    private Button refreshButton;
    private VisualElement bottomBar;
    private Toggle internalPlayerPrefsToggle;
    private Button newPlayerPrefButton;
    private Button applyButton;

    private IDictionary playerPrefs;
    private List<string> keysToDelete = new List<string>();

    [MenuItem("Essentials/PlayerPrefs Editor")]
    private static void ShowWindow()
    {
        PlayerPrefsEditorEditor window = GetWindow<PlayerPrefsEditorEditor>();
        window.titleContent = new GUIContent("PlayerPrefs Editor");
        window.minSize = new Vector2(300, 300);
    }

    public void CreateGUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/PlayerPrefsEditor/PlayerPrefsEditorEditorDocument.uxml");
        visualTree.CloneTree(rootVisualElement);

        playerPrefsList = rootVisualElement.Q<ScrollView>("PlayerPrefsList");
        noPlayerPrefsInfo = rootVisualElement.Q<VisualElement>("NoPlayerPrefsInfo");
        refreshButton = noPlayerPrefsInfo.Q<Button>("RefreshButton");
        bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
        internalPlayerPrefsToggle = bottomBar.Q<Toggle>("InternalPlayerPrefsToggle");
        newPlayerPrefButton = bottomBar.Q<Button>("NewPlayerPrefButton");
        applyButton = bottomBar.Q<Button>("ApplyButton");

        refreshButton.clicked += Reload;

        internalPlayerPrefsToggle.RegisterValueChangedCallback((_) => RefreshList());

        newPlayerPrefButton.clicked += () => PlayerPrefsAddEditor.ShowWindow(this);
        applyButton.clicked += Apply;

        applyButton.SetEnabled(false);

        Reload();
    }

    public void AddPlayerPref(string key, object value)
    {
        if (playerPrefs.Contains(key))
        {
            EditorUtility.DisplayDialog("Error", "Key already exists", "OK");
            return;
        }

        playerPrefs.Add(key, value);

        RefreshList();
        applyButton.SetEnabled(true);
    }

    private void Reload()
    {
        LoadPlayerPrefs();
        RefreshList();
    }

    private void LoadPlayerPrefs()
    {
#if PLATFORM_STANDALONE_OSX
        BinaryPlistReader reader = new BinaryPlistReader();
        playerPrefs = reader.ReadObject("/Users/" + Environment.UserName + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist");
#endif
    }

    private void RefreshList()
    {
#if PLATFORM_STANDALONE_OSX
        playerPrefsList.Clear();

        foreach (DictionaryEntry pair in playerPrefs)
        {
            if (!internalPlayerPrefsToggle.value && (pair.Key.ToString().StartsWith("unity.") || pair.Key.ToString() == "UnityGraphicsQuality")) continue;

            VisualElement element = new VisualElement();
            element.style.flexDirection = FlexDirection.Row;

            TextField valueField = new TextField();
            valueField.label = pair.Key.ToString();
            valueField.value = pair.Value.ToString();
            valueField.style.flexShrink = 1;
            valueField.style.flexGrow = 1;

            DropdownField typeField = new DropdownField();
            typeField.style.width = 100;
            typeField.choices = new List<string>() { "String", "Int", "Float" };

            if (pair.Value is string) typeField.value = "String";
            else if (pair.Value is Int16) typeField.value = "Int";
            else if (pair.Value is double) typeField.value = "Float";

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
                    playerPrefs[pair.Key] = Int16.Parse(valueField.value);
                }
                else if (typeField.value == "Float")
                {
                    if (!float.TryParse(valueField.value, out float value)) return;
                    playerPrefs[pair.Key] = double.Parse(valueField.value);
                }
                else playerPrefs[pair.Key] = valueField.value;
            });

            typeField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue == "Int")
                {
                    if (!int.TryParse(valueField.value, out int value)) valueField.value = "0";
                    playerPrefs[pair.Key] = Int16.Parse(valueField.value);
                }
                else if (evt.newValue == "Float")
                {
                    if (!float.TryParse(valueField.value, out float value)) valueField.value = "0";
                    playerPrefs[pair.Key] = double.Parse(valueField.value);
                }
                else playerPrefs[pair.Key] = valueField.value;

                applyButton.SetEnabled(true);
            });

            ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Delete PlayerPref", (_) =>
                {
                    keysToDelete.Add(pair.Key.ToString());
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
#endif

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
        PlayerPrefs.DeleteAll();

        foreach (DictionaryEntry pair in playerPrefs)
        {
            if (pair.Value is string) PlayerPrefs.SetString(pair.Key.ToString(), pair.Value.ToString());
            else if (pair.Value is Int16) PlayerPrefs.SetInt(pair.Key.ToString(), Int16.Parse(pair.Value.ToString()));
            else if (pair.Value is double) PlayerPrefs.SetFloat(pair.Key.ToString(), float.Parse(pair.Value.ToString()));
        }

        PlayerPrefs.Save();

        applyButton.SetEnabled(false);
    }
}