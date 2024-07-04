using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.GameSounds
{
    public class GameSoundsGroupEditor : EditorWindow
    {

        private GameSoundGroup _gameSoundGroup;

        private GameSoundsData _gameSoundsData;

        private SerializedObject _serializedObject;
        private SerializedProperty _gameSoundsGroupProperty;

        private PropertyField _groupNameField;

        private PropertyField _audioMixerGroupField;
        private PropertyField _volumeField;
        private PropertyField _loopField;
        private PropertyField _priorityField;
        private PropertyField _spatialBlendField;
        private PropertyField _spatializeField;
        private PropertyField _dopplerLevelField;
        private PropertyField _minDistanceField;
        private PropertyField _maxDistanceField;
        private PropertyField _panStereoField;
        private PropertyField _reverbZoneMixField;

        public static void CreateWindow(GameSoundGroup gameSoundGroup)
        {
            GameSoundsGroupEditor window = CreateInstance<GameSoundsGroupEditor>();
            window.titleContent = new GUIContent("Game Sounds Group Settings", EditorGUIUtility.IconContent("d_SceneViewAudio On").image);
            window.minSize = new Vector2(300, 300);

            window.SetGameSoundGroup(gameSoundGroup);
            window.Show();
        }

        private void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameSounds/GameSoundsGroupEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _gameSoundsData = GameSoundsSettings.GetData();

            _serializedObject = new SerializedObject(_gameSoundsData);

            int _gameSoundsGroupIndex = _gameSoundsData.gameSoundGroups.IndexOf(_gameSoundGroup);
            _gameSoundsGroupProperty = GetSerializedPropertyFromList(_gameSoundsGroupIndex);

            _groupNameField = rootVisualElement.Q<PropertyField>("GroupName");

            _audioMixerGroupField = rootVisualElement.Q<PropertyField>("AudioMixerGroupField");
            _volumeField = rootVisualElement.Q<PropertyField>("VolumeField");
            _loopField = rootVisualElement.Q<PropertyField>("LoopField");
            _priorityField = rootVisualElement.Q<PropertyField>("PriorityField");
            _spatialBlendField = rootVisualElement.Q<PropertyField>("SpatialBlendField");
            _spatializeField = rootVisualElement.Q<PropertyField>("SpatializeField");
            _dopplerLevelField = rootVisualElement.Q<PropertyField>("DopplerLevelField");
            _minDistanceField = rootVisualElement.Q<PropertyField>("MinDistanceField");
            _maxDistanceField = rootVisualElement.Q<PropertyField>("MaxDistanceField");
            _panStereoField = rootVisualElement.Q<PropertyField>("PanStereoField");
            _reverbZoneMixField = rootVisualElement.Q<PropertyField>("ReverbZoneMixField");

            BindProperties();
        }

        private void BindProperties()
        {
            SerializedProperty settingsProperty = _gameSoundsGroupProperty.FindPropertyRelative("settings");

            _groupNameField.BindProperty(_gameSoundsGroupProperty.FindPropertyRelative("name"));

            _audioMixerGroupField.BindProperty(settingsProperty.FindPropertyRelative("audioMixerGroup"));
            _volumeField.BindProperty(settingsProperty.FindPropertyRelative("volume"));
            _loopField.BindProperty(settingsProperty.FindPropertyRelative("loop"));
            _priorityField.BindProperty(settingsProperty.FindPropertyRelative("priority"));
            _spatialBlendField.BindProperty(settingsProperty.FindPropertyRelative("spatialBlend"));
            _spatializeField.BindProperty(settingsProperty.FindPropertyRelative("spatialize"));
            _dopplerLevelField.BindProperty(settingsProperty.FindPropertyRelative("dopplerLevel"));
            _minDistanceField.BindProperty(settingsProperty.FindPropertyRelative("minDistance"));
            _maxDistanceField.BindProperty(settingsProperty.FindPropertyRelative("maxDistance"));
            _panStereoField.BindProperty(settingsProperty.FindPropertyRelative("panStereo"));
            _reverbZoneMixField.BindProperty(settingsProperty.FindPropertyRelative("reverbZoneMix"));
        }

        private SerializedProperty GetSerializedPropertyFromList(int index)
        {
            SerializedProperty property = _serializedObject.FindProperty("gameSoundGroups");

            if (!property.isArray) return null;

            property.Next(true);
            property.Next(true);

            int listLength = property.intValue;

            for (int i = 0; i < listLength; i++)
            {
                property.Next(false);
                if (i == index) return property;
            }

            return null;
        }

        public void SetGameSoundGroup(GameSoundGroup gameSoundGroup) => _gameSoundGroup = gameSoundGroup;
    }
}