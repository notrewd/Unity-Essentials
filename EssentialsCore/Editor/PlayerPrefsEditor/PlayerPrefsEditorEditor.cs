using System;
using System.Collections;
using System.Runtime.Serialization.Plists;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPrefsEditorEditor : EditorWindow
{
    private ScrollView playerPrefsList;
    private VisualElement noPlayerPrefsInfo;
    private VisualElement bottomBar;
    private Button addKeyButton;
    private Button applyButton;

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
        bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
        addKeyButton = bottomBar.Q<Button>("AddKeyButton");
        applyButton = bottomBar.Q<Button>("ApplyButton");

        applyButton.SetEnabled(false);

#if PLATFORM_STANDALONE_OSX
        BinaryPlistReader reader = new BinaryPlistReader();
        IDictionary dict = reader.ReadObject("/Users/" + Environment.UserName + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist");

        if (dict.Count == 0) return;

        noPlayerPrefsInfo.style.display = DisplayStyle.None;
        playerPrefsList.style.display = DisplayStyle.Flex;

        foreach (DictionaryEntry pair in dict)
        {
            VisualElement element = new VisualElement();
            element.style.flexDirection = FlexDirection.Row;

            Label keyLabel = new Label(pair.Key.ToString());
            keyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            TextField valueField = new TextField();
            valueField.value = pair.Value.ToString();
            valueField.style.flexGrow = 1;
            valueField.RegisterValueChangedCallback((_) => applyButton.SetEnabled(true));

            element.Add(keyLabel);
            element.Add(valueField);
            playerPrefsList.Add(element);
        }
#endif
    }
}