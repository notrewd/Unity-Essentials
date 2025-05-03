using System;
using System.Collections.Generic;
using Essentials.Internal.GameSounds;
using UnityEngine;
using UnityEngine.Audio;

namespace Essentials.Core.GameSounds
{
    /// <summary>
    /// Represents a sound instance that can be configured and played.
    /// </summary>
    public class GameSound
    {
        private static GameSoundsData _gameSoundsData;
        private static GameSoundSettings _defaultSettings;
        private static HashSet<GameSound> _gameSounds = new HashSet<GameSound>();

        private string _id;

        private AudioSource _audioSource;
        private GameObject _gameObject;

        private AudioClip _audioClip;
        private Transform _parent;
        private Vector3 _position;
        private bool _isLocalPosition;
        private bool _worldPositionStays;
        private float _volume;
        private bool _loop;
        private int _priority;
        private float _spatialBlend;
        private bool _spatialize;
        private float _dopplerLevel;
        private float _minDistance;
        private float _maxDistance;
        private float _panStereo;
        private float _reverbZoneMix;
        private AudioMixerGroup _audioMixerGroup;
        private bool _mute;
        private bool _bypassEffects;
        private bool _bypassReverbZones;
        private bool _doNotDestroy;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _gameSoundsData = Resources.Load<GameSoundsData>("GameSoundsData");
            _defaultSettings = _gameSoundsData.defaultSettings;

            _gameSounds = GameSoundsController.GetGameSounds();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSound"/> class with the specified audio clip.
        /// Inherits default settings from GameSoundsData if available.
        /// </summary>
        /// <param name="audioClip">The audio clip to play.</param>
        public GameSound(AudioClip audioClip)
        {
            _audioClip = audioClip;
            _parent = null;
            _position = Vector3.zero;
            _isLocalPosition = true;
            _worldPositionStays = false;

            if (_gameSoundsData == null)
            {
                _volume = 1f;
                _loop = false;
                _priority = 128;
                _spatialBlend = 1f;
                _spatialize = true;
                _dopplerLevel = 1f;
                _minDistance = 1f;
                _maxDistance = 500f;
                _panStereo = 0f;
                _reverbZoneMix = 0f;
                _audioMixerGroup = null;
                _mute = false;
                _bypassEffects = false;
                _bypassReverbZones = false;
                _doNotDestroy = false;
            }
            else
            {
                _audioMixerGroup = _defaultSettings.audioMixerGroup;
                _mute = _defaultSettings.mute;
                _bypassEffects = _defaultSettings.bypassEffects;
                _bypassReverbZones = _defaultSettings.bypassReverbZones;
                _volume = _defaultSettings.volume;
                _loop = _defaultSettings.loop;
                _priority = _defaultSettings.priority;
                _spatialBlend = _defaultSettings.spatialBlend;
                _spatialize = _defaultSettings.spatialize;
                _dopplerLevel = _defaultSettings.dopplerLevel;
                _minDistance = _defaultSettings.minDistance;
                _maxDistance = _defaultSettings.maxDistance;
                _panStereo = _defaultSettings.panStereo;
                _reverbZoneMix = _defaultSettings.reverbZoneMix;
            }
        }

        /// <summary>
        /// Sets the unique identifier for this GameSound instance.
        /// If not set, a random GUID will be generated upon playing.
        /// </summary>
        /// <param name="id">The identifier string.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetId(string id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Sets the audio clip for this GameSound instance.
        /// </summary>
        /// <param name="audioClip">The audio clip to set.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
            if (_audioSource != null) _audioSource.clip = audioClip;

            return this;
        }

        /// <summary>
        /// Sets the parent transform for the GameSound's GameObject.
        /// </summary>
        /// <param name="parent">The parent transform.</param>
        /// <param name="worldPositionStays">If true, the object keeps its world position after parenting, otherwise its local position is relative to the new parent.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetParent(Transform parent, bool worldPositionStays = false)
        {
            _parent = parent;
            _worldPositionStays = worldPositionStays;

            if (_gameObject != null) _gameObject.transform.SetParent(parent, worldPositionStays);

            return this;
        }

        /// <summary>
        /// Sets the position of the GameSound's GameObject.
        /// </summary>
        /// <param name="position">The position vector.</param>
        /// <param name="isLocalPosition">If true, sets the local position relative to the parent; otherwise, sets the world position.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetPosition(Vector3 position, bool isLocalPosition = true)
        {
            _position = position;
            _isLocalPosition = isLocalPosition;

            if (_gameObject != null)
            {
                if (isLocalPosition) _gameObject.transform.localPosition = position;
                else _gameObject.transform.position = position;
            }

            return this;
        }

        /// <summary>
        /// Sets the volume of the GameSound.
        /// </summary>
        /// <param name="volume">The volume level (0.0 to 1.0).</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetVolume(float volume)
        {
            _volume = volume;
            if (_audioSource != null) _audioSource.volume = volume;

            return this;
        }

        /// <summary>
        /// Sets whether the GameSound should loop.
        /// </summary>
        /// <param name="loop">True to loop, false otherwise.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetLoop(bool loop)
        {
            _loop = loop;
            if (_audioSource != null) _audioSource.loop = loop;

            return this;
        }

        /// <summary>
        /// Sets the priority of the GameSound's AudioSource.
        /// </summary>
        /// <param name="priority">The priority level (0 = highest, 255 = lowest).</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetPriority(int priority)
        {
            _priority = priority;
            if (_audioSource != null) _audioSource.priority = priority;

            return this;
        }

        /// <summary>
        /// Sets the spatial blend of the GameSound's AudioSource (0.0 = 2D, 1.0 = 3D).
        /// </summary>
        /// <param name="spatialBlend">The spatial blend value.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetSpatialBlend(float spatialBlend)
        {
            _spatialBlend = spatialBlend;
            if (_audioSource != null) _audioSource.spatialBlend = spatialBlend;

            return this;
        }

        /// <summary>
        /// Enables or disables spatial blend (sets spatialBlend to 1.0 or 0.0).
        /// </summary>
        /// <param name="enabled">True to enable 3D spatial blend, false for 2D.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetSpatialBlend(bool enabled)
        {
            _spatialBlend = enabled ? 1 : 0;
            if (_audioSource != null) _audioSource.spatialBlend = enabled ? 1 : 0;

            return this;
        }

        /// <summary>
        /// Sets whether the GameSound's AudioSource should be spatialized.
        /// </summary>
        /// <param name="spatialize">True to spatialize, false otherwise.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetSpatialize(bool spatialize)
        {
            _spatialize = spatialize;
            if (_audioSource != null) _audioSource.spatialize = spatialize;

            return this;
        }

        /// <summary>
        /// Sets the doppler level of the GameSound's AudioSource.
        /// </summary>
        /// <param name="dopplerLevel">The doppler level.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetDopplerLevel(float dopplerLevel)
        {
            _dopplerLevel = dopplerLevel;
            if (_audioSource != null) _audioSource.dopplerLevel = dopplerLevel;

            return this;
        }

        /// <summary>
        /// Sets the minimum distance for the GameSound's 3D rolloff.
        /// </summary>
        /// <param name="minDistance">The minimum distance.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetMinDistance(float minDistance)
        {
            _minDistance = minDistance;
            if (_audioSource != null) _audioSource.minDistance = minDistance;

            return this;
        }

        /// <summary>
        /// Sets the maximum distance for the GameSound's 3D rolloff.
        /// </summary>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetMaxDistance(float maxDistance)
        {
            _maxDistance = maxDistance;
            if (_audioSource != null) _audioSource.maxDistance = maxDistance;

            return this;
        }

        /// <summary>
        /// Sets the stereo panning of the GameSound's AudioSource (-1.0 = left, 0.0 = center, 1.0 = right).
        /// </summary>
        /// <param name="panStereo">The stereo pan value.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetPanStereo(float panStereo)
        {
            _panStereo = panStereo;
            if (_audioSource != null) _audioSource.panStereo = panStereo;

            return this;
        }

        /// <summary>
        /// Sets the reverb zone mix level of the GameSound's AudioSource.
        /// </summary>
        /// <param name="reverbZoneMix">The reverb zone mix level (0.0 to 1.1).</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetReverbZoneMix(float reverbZoneMix)
        {
            _reverbZoneMix = reverbZoneMix;
            if (_audioSource != null) _audioSource.reverbZoneMix = reverbZoneMix;

            return this;
        }

        /// <summary>
        /// Sets the output AudioMixerGroup for the GameSound's AudioSource.
        /// </summary>
        /// <param name="audioMixerGroup">The AudioMixerGroup to output to.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            _audioMixerGroup = audioMixerGroup;
            if (_audioSource != null) _audioSource.outputAudioMixerGroup = audioMixerGroup;

            return this;
        }

        /// <summary>
        /// Sets whether the GameSound's AudioSource is muted.
        /// </summary>
        /// <param name="mute">True to mute, false otherwise.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetMute(bool mute)
        {
            _mute = mute;
            if (_audioSource != null) _audioSource.mute = mute;

            return this;
        }

        /// <summary>
        /// Sets whether the GameSound's AudioSource bypasses effects.
        /// </summary>
        /// <param name="bypassEffects">True to bypass effects, false otherwise.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetBypassEffects(bool bypassEffects)
        {
            _bypassEffects = bypassEffects;
            if (_audioSource != null) _audioSource.bypassEffects = bypassEffects;

            return this;
        }

        /// <summary>
        /// Sets whether the GameSound's AudioSource bypasses reverb zones.
        /// </summary>
        /// <param name="bypassReverbZones">True to bypass reverb zones, false otherwise.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetBypassReverbZones(bool bypassReverbZones)
        {
            _bypassReverbZones = bypassReverbZones;
            if (_audioSource != null) _audioSource.bypassReverbZones = bypassReverbZones;

            return this;
        }

        /// <summary>
        /// Sets whether the GameSound's GameObject should persist after the sound finishes playing.
        /// If false (default), the GameObject is destroyed automatically.
        /// </summary>
        /// <param name="doNotDestroy">True to prevent automatic destruction, false otherwise.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetDoNotDestroy(bool doNotDestroy)
        {
            _doNotDestroy = doNotDestroy;
            return this;
        }

        /// <summary>
        /// Applies settings from a predefined GameSoundGroup to this GameSound instance.
        /// Overwrites existing settings with the group's settings.
        /// </summary>
        /// <param name="groupName">The name of the GameSoundGroup to apply.</param>
        /// <returns>The current GameSound instance for chaining.</returns>
        public GameSound SetGroup(string groupName)
        {
            GameSoundGroup group = _gameSoundsData.GetGroup(groupName);

            if (group == null)
            {
                Debug.LogWarning($"Essentials GameSounds: Group '{groupName}' not found");
                return this;
            }

            _audioMixerGroup = group.settings.audioMixerGroup;
            _mute = group.settings.mute;
            _bypassEffects = group.settings.bypassEffects;
            _bypassReverbZones = group.settings.bypassReverbZones;
            _volume = group.settings.volume;
            _loop = group.settings.loop;
            _priority = group.settings.priority;
            _spatialBlend = group.settings.spatialBlend;
            _spatialize = group.settings.spatialize;
            _dopplerLevel = group.settings.dopplerLevel;
            _minDistance = group.settings.minDistance;
            _maxDistance = group.settings.maxDistance;
            _panStereo = group.settings.panStereo;
            _reverbZoneMix = group.settings.reverbZoneMix;

            return this;
        }

        /// <summary>
        /// Plays the GameSound. Creates the necessary GameObject and AudioSource if they don't exist.
        /// Applies all configured settings to the AudioSource.
        /// </summary>
        /// <returns>The current GameSound instance.</returns>
        public GameSound Play()
        {
            _id ??= Guid.NewGuid().ToString();

            if (_gameObject == null)
            {
                _gameObject = new GameObject($"GameSound [{_id}]");
                _gameObject.transform.SetParent(_parent, _worldPositionStays);

                if (_isLocalPosition) _gameObject.transform.localPosition = _position;
                else _gameObject.transform.position = _position;

                _gameSounds.Add(this);
            }

            if (_audioSource == null)
            {
                _audioSource = _gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.clip = _audioClip;

                _audioSource.volume = _volume;
                _audioSource.loop = _loop;
                _audioSource.priority = _priority;
                _audioSource.spatialBlend = _spatialBlend;
                _audioSource.spatialize = _spatialize;
                _audioSource.dopplerLevel = _dopplerLevel;
                _audioSource.minDistance = _minDistance;
                _audioSource.maxDistance = _maxDistance;
                _audioSource.panStereo = _panStereo;
                _audioSource.reverbZoneMix = _reverbZoneMix;
                _audioSource.outputAudioMixerGroup = _audioMixerGroup;
                _audioSource.mute = _mute;
                _audioSource.bypassEffects = _bypassEffects;
                _audioSource.bypassReverbZones = _bypassReverbZones;
            }

            _audioSource.Play();

            if (_audioSource.loop || _doNotDestroy) return this;

            GameSoundsController.StopSoundAfter(this, _audioClip.length);

            return this;
        }

        /// <summary>
        /// Stops the GameSound immediately.
        /// </summary>
        public void Stop() => _audioSource.Stop();

        /// <summary>
        /// Gets the unique identifier of this GameSound instance.
        /// </summary>
        /// <returns>The identifier string.</returns>
        public string GetId() => _id;

        /// <summary>
        /// Gets the GameObject associated with this GameSound instance.
        /// Note: The GameObject is created only when Play() is called.
        /// </summary>
        /// <returns>The associated GameObject, or null if Play() hasn't been called.</returns>
        public GameObject GetGameObject() => _gameObject;

        /// <summary>
        /// Gets the AudioSource component associated with this GameSound instance.
        /// Note: The AudioSource is created only when Play() is called.
        /// </summary>
        /// <returns>The associated AudioSource, or null if Play() hasn't been called.</returns>
        public AudioSource GetAudioSource() => _audioSource;
    }
}