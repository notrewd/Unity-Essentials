using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectoriesSettingsEditor : EditorWindow
    {
        private static Texture _settingsTexture;

        private VisualElement bottomBar;
        private VisualElement content;
        private Button applyButton;
        private TextField classNameField;
        private Label classLocationLabel;
        private Button classLocationButton;
        private VisualElement directoryReferences;
        private VisualElement unappliedChanges;

        private static void SetTextures()
        {
            _settingsTexture = EditorGUIUtility.isProSkin ? EditorGUIUtility.IconContent("d_Settings").image : EditorGUIUtility.IconContent("Settings").image;
        }

        public static GameDirectoriesSettingsEditor Open()
        {
            SetTextures();

            GameDirectoriesSettingsEditor window = GetWindow<GameDirectoriesSettingsEditor>();
            window.titleContent = new GUIContent("Game Directories Settings", _settingsTexture);
            window.minSize = new Vector2(300, 300);

            return window;
        }

        public void CreateGUI()
        {
            GameDirectoriesSettings.LoadData();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameDirectories/GameDirectoriesSettingsEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            content = rootVisualElement.Q<VisualElement>("Content");
            bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            applyButton = bottomBar.Q<Button>("ApplyButton");
            classNameField = content.Q("ClassName").Q<TextField>("ClassNameField");
            classLocationLabel = content.Q("ClassLocation").Q<Label>("ClassLocationLabel");
            classLocationButton = content.Q("ClassLocation").Q<Button>("ClassLocationButton");
            directoryReferences = content.Q<VisualElement>("DirectoryReferences");
            unappliedChanges = rootVisualElement.Q<VisualElement>("UnappliedChanges");

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

            rootVisualElement.RegisterCallback<FocusInEvent>(_ =>
            {
                if (!GameDirectoriesEditor.Instance.appliedChanges) unappliedChanges.style.display = DisplayStyle.Flex;
                else unappliedChanges.style.display = DisplayStyle.None;
            });

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

                if (directoryReferences.Children().Count(x => x != directoryReference && x is TextField && ((TextField)x).value == referenceField.value) > 0) return false;
            }

            return true;
        }

        public void Refresh()
        {
            applyButton.SetEnabled(false);

            classNameField.value = GameDirectoriesSettings.GetClassName();
            classLocationLabel.text = GameDirectoriesSettings.GetClassLocation();

            directoryReferences.Clear();

            GameDirectoryData[] gameDirectoriesData = GameDirectoriesSettings.GetGameDirectoriesData();

            if (gameDirectoriesData == null || gameDirectoriesData.Length == 0)
            {
                Label label = new Label("No directories have been added yet.");
                label.style.color = Color.grey;

                directoryReferences.Add(label);
            }
            else
            {
                foreach (GameDirectoryData gameDirectoryData in gameDirectoriesData)
                {
                    TextField referenceField = new TextField
                    {
                        label = gameDirectoryData.path,
                        value = gameDirectoryData.reference
                    };

                    referenceField.RegisterValueChangedCallback(_ =>
                    {
                        applyButton.SetEnabled(Validate());
                    });

                    directoryReferences.Add(referenceField);
                }
            }

            if (!GameDirectoriesEditor.Instance.appliedChanges) unappliedChanges.style.display = DisplayStyle.Flex;
            else unappliedChanges.style.display = DisplayStyle.None;
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

                GameDirectoriesEditor.Instance.FindGameDirectory(referenceField.label).reference = referenceField.value;
            }

            GameDirectory[] gameDirectories = GameDirectoriesEditor.Instance.GetAllGameDirectories();
            GameDirectoryData[] gameDirectoriesData = new GameDirectoryData[gameDirectories.Length];

            for (int i = 0; i < gameDirectories.Length; i++)
            {
                gameDirectoriesData[i] = new GameDirectoryData(gameDirectories[i].path, gameDirectories[i].reference);
            }

            GameDirectoriesSettings.SetGameDirectoriesData(gameDirectoriesData);
            GameDirectoriesSettings.SaveData();
            GameDirectoriesSettings.GenerateClass(gameDirectories);
        }
    }
}