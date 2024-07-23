using System.Linq;
using Essentials.Inspector.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectoriesSettingsEditor : EditorWindow
    {
        private VisualElement _bottomBar;
        private VisualElement _content;
        private Button _applyButton;
        private TextField _classNameField;
        private Label _classLocationLabel;
        private Button _classLocationButton;
        private VisualElement _directoryReferences;
        private VisualElement _unappliedChanges;

        public static GameDirectoriesSettingsEditor Open()
        {
            GameDirectoriesSettingsEditor window = GetWindow<GameDirectoriesSettingsEditor>();
            window.titleContent = new GUIContent("Game Directories Settings", IconDatabase.GetIcon("Settings@32"));
            window.minSize = new Vector2(300, 300);

            return window;
        }

        public void CreateGUI()
        {
            GameDirectoriesSettings.LoadData();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameDirectories/GameDirectoriesSettingsEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _content = rootVisualElement.Q<VisualElement>("Content");
            _bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");
            _applyButton = _bottomBar.Q<Button>("ApplyButton");
            _classNameField = _content.Q("ClassName").Q<TextField>("ClassNameField");
            _classLocationLabel = _content.Q("ClassLocation").Q<Label>("ClassLocationLabel");
            _classLocationButton = _content.Q("ClassLocation").Q<Button>("ClassLocationButton");
            _directoryReferences = _content.Q<VisualElement>("DirectoryReferences");
            _unappliedChanges = rootVisualElement.Q<VisualElement>("UnappliedChanges");

            _applyButton.SetEnabled(false);

            _applyButton.clickable.clicked += Apply;

            _classNameField.RegisterValueChangedCallback(_ =>
            {
                _applyButton.SetEnabled(Validate());
            });

            _classLocationButton.clickable.clicked += () =>
            {
                string path = EditorUtility.OpenFolderPanel("Select Class Location", "Assets", "");
                if (string.IsNullOrWhiteSpace(path)) return;

                if (!path.StartsWith(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Invalid Path", "The selected path is not a valid path within the project.", "OK");
                    return;
                }

                _classLocationLabel.text = "Assets" + path[Application.dataPath.Length..];
                _applyButton.SetEnabled(Validate());
            };

            rootVisualElement.RegisterCallback<FocusInEvent>(_ =>
            {
                if (!GameDirectoriesEditor.Instance.appliedChanges) _unappliedChanges.style.display = DisplayStyle.Flex;
                else _unappliedChanges.style.display = DisplayStyle.None;
            });

            Refresh();
        }

        private bool Validate()
        {
            string[] reservedNames = new string[]
            {
                "GameDirectory", "GameDirectories", "GameDirectoriesSettings", "GameDirectoriesSettingsEditor", "GameDirectoriesEditor", "GameDirectoriesPopup"
            };

            if (string.IsNullOrWhiteSpace(_classNameField.value)) return false;
            if (reservedNames.Contains(_classNameField.value)) return false;
            if (!char.IsLetter(_classNameField.value[0])) return false;
            foreach (char character in _classNameField.value) if (!char.IsLetterOrDigit(character) && character != '_') return false;

            foreach (VisualElement directoryReference in _directoryReferences.Children())
            {
                if (directoryReference is Label) continue;

                TextField referenceField = directoryReference.Q<TextField>();
                if (string.IsNullOrEmpty(referenceField.value)) continue;
                if (!char.IsLetter(referenceField.value[0])) return false;
                foreach (char character in referenceField.value) if (!char.IsLetterOrDigit(character) && character != '_') return false;

                if (_directoryReferences.Children().Count(x => x != directoryReference && x is TextField && ((TextField)x).value == referenceField.value) > 0) return false;
            }

            return true;
        }

        public void Refresh()
        {
            _applyButton.SetEnabled(false);

            _classNameField.value = GameDirectoriesSettings.GetClassName();
            _classLocationLabel.text = GameDirectoriesSettings.GetClassLocation();

            _directoryReferences.Clear();

            GameDirectoryData[] gameDirectoriesData = GameDirectoriesSettings.GetGameDirectoriesData();

            if (gameDirectoriesData == null || gameDirectoriesData.Length == 0)
            {
                Label label = new Label("No directories have been added yet.");
                label.style.color = Color.grey;

                _directoryReferences.Add(label);
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
                        _applyButton.SetEnabled(Validate());
                    });

                    _directoryReferences.Add(referenceField);
                }
            }

            if (!GameDirectoriesEditor.Instance.appliedChanges) _unappliedChanges.style.display = DisplayStyle.Flex;
            else _unappliedChanges.style.display = DisplayStyle.None;
        }

        public void Apply()
        {
            _applyButton.SetEnabled(false);

            GameDirectoriesSettings.SetClassName(_classNameField.value);
            GameDirectoriesSettings.SetClassLocation(_classLocationLabel.text);

            foreach (VisualElement directoryReference in _directoryReferences.Children())
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