using System.Linq;
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

            Refresh();
        }

        private bool Validate()
        {
            string[] reservedNames = new string[]
            {
                "GameDirectory", "GameDirectories", "GameDirectoriesSettings", "GameDirectoriesSettingsEditor", "GameDirectoriesEditor", "GameDirectoriesPopup"
            };

            if (string.IsNullOrWhiteSpace(classNameField.value)) return false;
            if (reservedNames.Contains(classNameField.value)) return false;
            if (!char.IsLetter(classNameField.value[0])) return false;
            foreach (char character in classNameField.value) if (!char.IsLetterOrDigit(character) && character != '_') return false;

            foreach (VisualElement directoryReference in directoryReferences.Children())
            {
                if (directoryReference is Label) continue;

                TextField referenceField = directoryReference.Q<TextField>();
                if (string.IsNullOrEmpty(referenceField.value)) continue;
                if (!char.IsLetter(referenceField.value[0])) return false;
                foreach (char character in referenceField.value) if (!char.IsLetterOrDigit(character) && character != '_') return false;
            }

            return true;
        }

        public void Refresh()
        {
            applyButton.SetEnabled(false);

            classNameField.value = GameDirectoriesSettings.GetClassName();
            classLocationLabel.text = GameDirectoriesSettings.GetClassLocation();

            directoryReferences.Clear();

            // load directories from settings in format "path1,reference1;path2,reference2;..."
            string directories = GameDirectoriesSettings.GetGameDirectories();

            if (directories.Length == 0)
            {
                Label label = new Label("No directories have been added yet.");
                label.style.color = Color.grey;

                directoryReferences.Add(label);
            }
            else
            {
                string[] directoriesData = directories.Split(';');

                foreach (string directoryData in directoriesData)
                {
                    string[] directoryDataSplit = directoryData.Split(',');
                    if (directoryDataSplit.Length != 2) continue;

                    string path = directoryDataSplit[0];
                    string reference = directoryDataSplit[1];

                    TextField referenceField = new TextField();

                    referenceField.value = reference;
                    referenceField.label = path;

                    referenceField.RegisterValueChangedCallback(_ =>
                    {
                        applyButton.SetEnabled(Validate());
                    });

                    directoryReferences.Add(referenceField);
                }
            }
        }

        public void Apply()
        {
            applyButton.SetEnabled(false);

            GameDirectoriesSettings.SetClassName(classNameField.value);
            GameDirectoriesSettings.SetClassLocation(classLocationLabel.text);

            foreach (VisualElement directoryReference in directoryReferences.Children())
            {
                if (directoryReference is Label) continue;

                TextField referenceField = directoryReference.Q<TextField>();

                Core.GameDirectories.GameDirectories.FindGameDirectory(referenceField.label).reference = referenceField.value;
            }

            GameDirectory[] gameDirectories = Core.GameDirectories.GameDirectories.GetAllGameDirectories();

            GameDirectoriesSettings.SetGameDirectories(string.Join(";", gameDirectories.Select(x => $"{x.path},{x.reference}")));

            GameDirectoriesSettings.GenerateClass(gameDirectories);
        }
    }
}