using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Essentials.Internal.GameSounds;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace Essentials.Core.GameSounds
{
    public static class GameSounds
    {
        public static GameSound CreateSound(AudioClip audioClip) => new(audioClip);
        public static GameSound PlaySound(AudioClip audioClip) => CreateSound(audioClip).Play();
        public static GameSound PlaySound(string id, AudioClip audioClip) => CreateSound(audioClip).SetId(id).Play();

        public static void StopSound(string id)
        {
            foreach (GameSound gameSound in GameSoundsController.GetGameSounds().Where(gameSound => gameSound.GetId() == id)) gameSound.Destroy();
        }

        public static void StopAllSounds()
        {
            foreach (GameSound gameSound in GameSoundsController.GetGameSounds()) gameSound.Destroy();
        }

        public static bool IsPlaying(string id)
        {
            foreach (GameSound gameSound in GameSoundsController.GetGameSounds())
            {
                if (gameSound.GetId() == id) return true;
            }

            return false;
        }
    }

    public class GameSound
    {
        private static GameSoundsData _gameSoundsData;
        private static HashSet<GameSound> _gameSounds = new HashSet<GameSound>();

        private string _id;

        private AudioSource _audioSource;
        private GameObject _gameObject;

        private AudioClip _audioClip;
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
        private bool _doNotDestroy;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _gameSoundsData = Resources.Load<GameSoundsData>("GameSoundsData");
            _gameSounds = GameSoundsController.GetGameSounds();
        }

        public GameSound(AudioClip audioClip)
        {
            _audioClip = audioClip;

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
                _doNotDestroy = false;
            }
            else
            {
                _volume = _gameSoundsData.volume;
                _loop = _gameSoundsData.loop;
                _priority = _gameSoundsData.priority;
                _spatialBlend = _gameSoundsData.spatialBlend;
                _spatialize = _gameSoundsData.spatialize;
                _dopplerLevel = _gameSoundsData.dopplerLevel;
                _minDistance = _gameSoundsData.minDistance;
                _maxDistance = _gameSoundsData.maxDistance;
                _panStereo = _gameSoundsData.panStereo;
                _reverbZoneMix = _gameSoundsData.reverbZoneMix;
            }
        }

        public GameSound SetId(string id)
        {
            _id = id;
            return this;
        }

        public GameSound SetClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
            if (_audioSource != null) _audioSource.clip = audioClip;

            return this;
        }

        public GameSound SetParent(Transform parent, bool worldPositionStays = false)
        {
            _gameObject.transform.SetParent(parent, worldPositionStays);
            return this;
        }

        public GameSound SetPosition(Vector3 position)
        {
            _gameObject.transform.position = position;
            return this;
        }

        public GameSound SetVolume(float volume)
        {
            _volume = volume;
            if (_audioSource != null) _audioSource.volume = volume;

            return this;
        }

        public GameSound SetLoop(bool loop)
        {
            _loop = loop;
            if (_audioSource != null) _audioSource.loop = loop;

            return this;
        }

        public GameSound SetPriority(int priority)
        {
            _priority = priority;
            if (_audioSource != null) _audioSource.priority = priority;

            return this;
        }

        public GameSound SetSpatialBlend(float spatialBlend)
        {
            _spatialBlend = spatialBlend;
            if (_audioSource != null) _audioSource.spatialBlend = spatialBlend;

            return this;
        }

        public GameSound SetSpatialBlend(bool enabled)
        {
            _spatialBlend = enabled ? 1 : 0;
            if (_audioSource != null) _audioSource.spatialBlend = enabled ? 1 : 0;

            return this;
        }

        public GameSound SetSpatialize(bool spatialize)
        {
            _spatialize = spatialize;
            if (_audioSource != null) _audioSource.spatialize = spatialize;

            return this;
        }

        public GameSound SetDopplerLevel(float dopplerLevel)
        {
            _dopplerLevel = dopplerLevel;
            if (_audioSource != null) _audioSource.dopplerLevel = dopplerLevel;

            return this;
        }

        public GameSound SetMinDistance(float minDistance)
        {
            _minDistance = minDistance;
            if (_audioSource != null) _audioSource.minDistance = minDistance;

            return this;
        }

        public GameSound SetMaxDistance(float maxDistance)
        {
            _maxDistance = maxDistance;
            if (_audioSource != null) _audioSource.maxDistance = maxDistance;

            return this;
        }

        public GameSound SetPanStereo(float panStereo)
        {
            _panStereo = panStereo;
            if (_audioSource != null) _audioSource.panStereo = panStereo;

            return this;
        }

        public GameSound SetReverbZoneMix(float reverbZoneMix)
        {
            _reverbZoneMix = reverbZoneMix;
            if (_audioSource != null) _audioSource.reverbZoneMix = reverbZoneMix;

            return this;
        }

        public GameSound SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            _audioMixerGroup = audioMixerGroup;
            if (_audioSource != null) _audioSource.outputAudioMixerGroup = audioMixerGroup;

            return this;
        }

        public GameSound SetDoNotDestroy(bool doNotDestroy)
        {
            _doNotDestroy = doNotDestroy;
            return this;
        }

        public GameSound Play()
        {
            _id ??= Guid.NewGuid().ToString();

            if (_gameObject == null)
            {
                _gameObject = new GameObject($"GameSound [{_id}]");
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
            }

            _audioSource.Play();

            if (_audioSource.loop || _doNotDestroy) return this;

            GameSoundsController.StopSoundAfter(this, _audioClip.length);

            return this;
        }

        public void Stop() => _audioSource.Stop();

        public string GetId() => _id;
        public AudioSource GetAudioSource() => _audioSource;

        public void Destroy()
        {
            _gameSounds.Remove(this);
            Object.Destroy(_gameObject);
        }

        public void DestroyImmediate()
        {
            _gameSounds.Remove(this);
            Object.DestroyImmediate(_gameObject);
        }
    }
}