using Essentials.Core.GameDirectories;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectoriesSettingsEditor : EditorWindow
    {
        private VisualElement bottomBar;
        private VisualElement content;
        private Button applyButton;
        private TextField classNameField;
        private Label classLocationLabel;
        private Button classLocationButton;
        private VisualElement directoryReferences;

        public void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameDirectories/GameDirectoriesSettingsEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            content = rootVisualElement.Q<VisualElement>("Content");
            bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            applyButton = bottomBar.Q<Button>("ApplyButton");
            classNameField = content.Q("ClassName").Q<TextField>("ClassNameField");
            classLocationLabel = content.Q("ClassLocation").Q<Label>("ClassLocationLabel");
            classLocationButton = content.Q("ClassLocation").Q<Button>("ClassLocationButton");
            directoryReferences = content.Q<VisualElement>("DirectoryReferences");

            applyButton.SetEnabled(false);

            applyButton.clickable.clicked += Apply;

            classNameField.RegisterValueChangedCallback(_ =>
            {
                applyButton.SetEnabled(Validate());
            });

            classLocationButton.clickable.clicked += () =>
            {
                string path = EditorUtility.OpenFolderPanel("Select Class Location", "Assets", "");
                if (string.IsNullOrWhiteSpace(path)) return;

                if (!path.StartsWith(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Invalid Path", "The selected path is not a valid path within the project.", "OK");
                    return;
                }

                classLocationLabel.text = "Assets" + path[Application.dataPath.Length..];
                applyButton.SetEnabled(Validate());
            };
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(classNameField.value)) return false;
            if (!char.IsLetter(classNameField.value[0])) return false;
            foreach (char character in classNameField.value) if (!char.IsLetterOrDigit(character) && character != '_') return false;

            return true;
        }

        public void Refresh()
        {
            applyButton.SetEnabled(false);
        }

        public void Apply()
        {
            applyButton.SetEnabled(false);
        }
    }
}