using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.GameSounds
{
    public class GameSoundsEditor : EditorWindow
    {
        private SerializedObject _serializedObject;
        private GameSoundsData _gameSoundsData;

        private VisualElement bottomBar;
        private Button resetButton;

        [MenuItem("Essentials/Game Sounds")]
        private static void ShowWindow()
        {
            EditorWindow window = GetWindow<GameSoundsEditor>();
            window.titleContent = new GUIContent("Game Sounds", EditorGUIUtility.IconContent("d_SceneViewAudio On").image);
            window.minSize = new Vector2(300, 300);
        }

        public void CreateGUI()
        {
            _gameSoundsData = GameSoundsSettings.GetData();
            _serializedObject = new SerializedObject(_gameSoundsData);

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameSounds/GameSoundsEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            bottomBar = rootVisualElement.Q<VisualElement>("BottomBar");

            resetButton = bottomBar.Q<Button>("ResetButton");
            resetButton.clicked += OnResetButtonClicked;

            rootVisualElement.Bind(_serializedObject);
        }

        public void OnResetButtonClicked()
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
