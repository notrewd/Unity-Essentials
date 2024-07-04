using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.GameSounds
{
    public class GameSoundsEditor : EditorWindow
    {
        private VisualTreeAsset _gameSoundGroupTemplate;

        private GameSoundsData _gameSoundsData;
        private SerializedObject _serializedObject;

        private VisualElement _bottomBar;

        private Foldout _groupsList;
        private VisualElement _newGroupElement;
        private Button _newGroupButton;

        private Button _resetButton;

        [MenuItem("Essentials/Game Sounds")]
        private static void ShowWindow()
        {
            EditorWindow window = GetWindow<GameSoundsEditor>();
            window.titleContent = new GUIContent("Game Sounds", EditorGUIUtility.IconContent("d_SceneViewAudio On").image);
            window.minSize = new Vector2(300, 300);
        }

        private void CreateGUI()
        {
            _gameSoundsData = GameSoundsSettings.GetData();
            _serializedObject = new SerializedObject(_gameSoundsData);

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameSounds/GameSoundsEditorDocument.uxml");
            _gameSoundGroupTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameSounds/GameSoundsGroupDocument.uxml");

            visualTree.CloneTree(rootVisualElement);

            _groupsList = rootVisualElement.Q<Foldout>("GroupsList");

            _newGroupElement = _groupsList.Q<VisualElement>("NewGroupElement");

            _newGroupButton = _newGroupElement.Q<Button>("NewGroupButton");
            _newGroupButton.clicked += AddGroup;

            _bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");

            _resetButton = _bottomBar.Q<Button>("ResetButton");
            _resetButton.clicked += OnResetButtonClicked;

            rootVisualElement.Bind(_serializedObject);

            RefreshGroups();

            RebindWindowEvents();
        }

        private void RebindWindowEvents()
        {
            GameSoundsGroupEditor[] windows = GameSoundsGroupEditor.GetActiveWindows();
            foreach (GameSoundsGroupEditor window in windows) window.onGroupNameChanged += RefreshGroups;
        }

        private void RefreshGroups()
        {
            _groupsList.Clear();

            for (int i = 0; i < _gameSoundsData.gameSoundGroups.Count; i++)
            {
                GameSoundGroup gameSoundGroup = _gameSoundsData.gameSoundGroups[i];

                VisualElement gameSoundGroupElement = _gameSoundGroupTemplate.Instantiate();

                Label groupName = gameSoundGroupElement.Q<Label>("GroupName");
                VisualElement buttons = gameSoundGroupElement.Q<VisualElement>("Buttons");

                Button editButton = buttons.Q<Button>("EditButton");
                Button deleteButton = buttons.Q<Button>("DeleteButton");

                if (i % 2 == 1) gameSoundGroupElement.style.backgroundColor = (Color)new Color32(47, 47, 47, 255);

                editButton.clicked += () => OpenGroup(gameSoundGroup);
                deleteButton.clicked += () => RemoveGroup(gameSoundGroup);

                groupName.text = gameSoundGroup.name;

                _groupsList.Add(gameSoundGroupElement);
            }

            _groupsList.Add(_newGroupElement);
        }

        private void AddGroup()
        {
            GameSoundGroup gameSoundGroup = new GameSoundGroup();
            _gameSoundsData.gameSoundGroups.Add(gameSoundGroup);
            EditorUtility.SetDirty(_gameSoundsData);

            RefreshGroups();
        }

        private void OpenGroup(GameSoundGroup gameSoundGroup)
        {
            GameSoundsGroupEditor window = GameSoundsGroupEditor.CreateWindow(gameSoundGroup);
            window.onGroupNameChanged += RefreshGroups;
        }

        private void RemoveGroup(GameSoundGroup gameSoundGroup)
        {
            if (!EditorUtility.DisplayDialog("Delete", "Are you sure you want to delete this group?", "Yes", "No")) return;

            _gameSoundsData.gameSoundGroups.Remove(gameSoundGroup);
            EditorUtility.SetDirty(_gameSoundsData);

            RefreshGroups();
        }

        private void OnResetButtonClicked()
        {
            if (!EditorUtility.DisplayDialog("Reset", "Are you sure you want to reset the sound settings?", "Yes", "No")) return;

            rootVisualElement.Unbind();

            GameSoundsSettings.ResetData();

            _gameSoundsData = GameSoundsSettings.GetData();
            _serializedObject = new SerializedObject(_gameSoundsData);

            rootVisualElement.Bind(_serializedObject);
        }
    }
}
