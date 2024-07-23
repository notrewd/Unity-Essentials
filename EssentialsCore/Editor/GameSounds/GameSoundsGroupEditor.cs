using System;
using System.Collections.Generic;
using Essentials.Inspector.Utilities;
using Essentials.Serialization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Essentials.Internal.GameSounds
{
    public class GameSoundsGroupEditor : EditorWindow
    {
        private static List<GameSoundsGroupEditor> _windows = new List<GameSoundsGroupEditor>();

        public Action onGroupNameChanged;

        private GameSoundGroup _gameSoundGroup;

        private GameSoundsData _gameSoundsData;

        private SerializedObject _serializedObject;
        private SerializedProperty _gameSoundsGroupProperty;

        private PropertyField _groupNameField;

        private PropertyField _audioMixerGroupField;
        private PropertyField _muteField;
        private PropertyField _bypassEffectsField;
        private PropertyField _bypassListenerEffectsField;
        private PropertyField _bypassReverbZonesField;
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

        public static GameSoundsGroupEditor CreateWindow(GameSoundGroup gameSoundGroup)
        {
            GameSoundsGroupEditor window = CreateInstance<GameSoundsGroupEditor>();
            window.titleContent = new GUIContent("Game Sounds Group Settings", IconDatabase.GetIcon("Settings@32"));
            window.minSize = new Vector2(300, 300);

            window.SetGameSoundGroup(gameSoundGroup);
            window.Show();

            return window;
        }

        private void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.notrewd.essentials/EssentialsCore/Editor/GameSounds/GameSoundsGroupEditorDocument.uxml");
            visualTree.CloneTree(rootVisualElement);

            _gameSoundsData = GameSoundsSettings.GetData();

            int _gameSoundsGroupIndex = _gameSoundsData.gameSoundGroups.IndexOf(_gameSoundGroup);

            _serializedObject = new SerializedObject(_gameSoundsData);
            SerializedProperty property = _serializedObject.FindProperty("gameSoundGroups");

            _gameSoundsGroupProperty = EssentialsSerialization.GetSerializedPropertyFromList(property, _gameSoundsGroupIndex);

            _groupNameField = rootVisualElement.Q<PropertyField>("GroupName");

            _groupNameField.RegisterValueChangeCallback((evt) => onGroupNameChanged?.Invoke());

            _audioMixerGroupField = rootVisualElement.Q<PropertyField>("AudioMixerGroupField");
            _muteField = rootVisualElement.Q<PropertyField>("MuteField");
            _bypassEffectsField = rootVisualElement.Q<PropertyField>("BypassEffectsField");
            _bypassListenerEffectsField = rootVisualElement.Q<PropertyField>("BypassListenerEffectsField");
            _bypassReverbZonesField = rootVisualElement.Q<PropertyField>("BypassReverbZonesField");
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

            _audioMixerGroupField.RegisterValueChangeCallback((evt) =>
            {
                _bypassListenerEffectsField.SetEnabled(evt.changedProperty.objectReferenceValue == null);
            });

            BindProperties();

            _windows.Add(this);
        }

        private void OnDestroy() => _windows.Remove(this);

        private void BindProperties()
        {
            SerializedProperty settingsProperty = _gameSoundsGroupProperty.FindPropertyRelative("settings");

            _groupNameField.BindProperty(_gameSoundsGroupProperty.FindPropertyRelative("name"));

            _audioMixerGroupField.BindProperty(settingsProperty.FindPropertyRelative("audioMixerGroup"));
            _muteField.BindProperty(settingsProperty.FindPropertyRelative("mute"));
            _bypassEffectsField.BindProperty(settingsProperty.FindPropertyRelative("bypassEffects"));
            _bypassListenerEffectsField.BindProperty(settingsProperty.FindPropertyRelative("bypassListenerEffects"));
            _bypassReverbZonesField.BindProperty(settingsProperty.FindPropertyRelative("bypassReverbZones"));
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

        public void SetGameSoundGroup(GameSoundGroup gameSoundGroup) => _gameSoundGroup = gameSoundGroup;

        public static GameSoundsGroupEditor[] GetActiveWindows() => _windows.ToArray();
    }
}