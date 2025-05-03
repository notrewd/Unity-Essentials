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
    /// <summary>
    /// Editor window for editing the settings of a specific GameSoundGroup.
    /// </summary>
    public class GameSoundsGroupEditor : EditorWindow
    {
        private static List<GameSoundsGroupEditor> _windows = new List<GameSoundsGroupEditor>();

        /// <summary>
        /// Event triggered when the group name changes.
        /// </summary>
        public event Action onGroupNameChanged;

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

        /// <summary>
        /// Creates and shows a new GameSoundsGroupEditor window for the specified group.
        /// </summary>
        /// <param name="gameSoundGroup">The GameSoundGroup to edit.</param>
        /// <returns>The created GameSoundsGroupEditor window instance.</returns>
        public static GameSoundsGroupEditor CreateWindow(GameSoundGroup gameSoundGroup)
        {
            GameSoundsGroupEditor window = CreateInstance<GameSoundsGroupEditor>();
            window.titleContent = new GUIContent("Game Sounds Group Settings", IconDatabase.GetIcon("Settings@32"));
            window.minSize = new Vector2(300, 300);

            window.SetGameSoundGroup(gameSoundGroup);
            window.Show();

            return window;
        }

        /// <summary>
        /// Creates the GUI for the editor window.
        /// </summary>
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

        /// <summary>
        /// Called when the editor window is destroyed.
        /// Removes the window from the list of active windows.
        /// </summary>
        private void OnDestroy() => _windows.Remove(this);

        /// <summary>
        /// Binds the UI elements to the serialized properties of the GameSoundGroup.
        /// </summary>
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

        /// <summary>
        /// Sets the GameSoundGroup to be edited by this window.
        /// </summary>
        /// <param name="gameSoundGroup">The GameSoundGroup to edit.</param>
        public void SetGameSoundGroup(GameSoundGroup gameSoundGroup) => _gameSoundGroup = gameSoundGroup;

        /// <summary>
        /// Gets an array of all currently active GameSoundsGroupEditor windows.
        /// </summary>
        /// <returns>An array of active GameSoundsGroupEditor windows.</returns>
        public static GameSoundsGroupEditor[] GetActiveWindows() => _windows.ToArray();
    }
}