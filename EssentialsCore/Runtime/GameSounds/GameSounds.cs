using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Essentials.Core.GameSounds
{
    public static class GameSounds
    {
        private class GameSound
        {
            public string id { get; }
            public AudioSource audioSource { get; }

            private readonly GameObject _gameObject;
        
            public GameSound(string id, AudioClip audioClip, Vector3 position = default, Transform parent = null, bool looping = false, float volume = 1, float spatialBlend = 0)
            {
                this.id = id;
                
                _gameObject = new GameObject($"GameSound [{id}]");
                if (parent != null) _gameObject.transform.SetParent(parent, false);
                _gameObject.transform.localPosition = position;
            
                audioSource = _gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.clip = audioClip;
                audioSource.loop = looping;
                audioSource.volume = volume;
                audioSource.spatialBlend = spatialBlend;
                audioSource.Play();
            }

            public void Destroy() => Object.Destroy(_gameObject);
        }
        
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static CancellationToken _cancellationToken;
        
        private static readonly HashSet<GameSound> _gameSounds = new HashSet<GameSound>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _cancellationToken = _cancellationTokenSource.Token;
            Application.quitting += OnApplicationQuit;
        }

        public static AudioSource PlaySound(AudioClip audioClip, Transform parent = null, float volume = 1, float spatialBlend = 0) => PlaySound("Essentials.NoId", audioClip, default, parent, false, volume, spatialBlend);
        public static AudioSource PlaySound(AudioClip audioClip, Transform parent, bool looping, float volume = 1, float spatialBlend = 0) => PlaySound("Essentials.NoId", audioClip, default, parent, looping, volume, spatialBlend);
        public static AudioSource PlaySound(AudioClip audioClip, Vector3 position, float volume = 1, float spatialBlend = 0) => PlaySound("Essentials.NoId", audioClip, position, null, false, volume, spatialBlend);
        public static AudioSource PlaySound(AudioClip audioClip, Vector3 position, Transform parent, float volume = 1, float spatialBlend = 0) => PlaySound("Essentials.NoId", audioClip, position, parent, false, volume, spatialBlend);
        public static AudioSource PlaySound(AudioClip audioClip, Vector3 position, bool looping = false, float volume = 1, float spatialBlend = 0) => PlaySound("Essentials.NoId", audioClip, position, null, looping, volume, spatialBlend);
        public static AudioSource PlaySound(AudioClip audioClip, float volume, float spatialBlend = 0) => PlaySound("Essentials.NoId", audioClip, default, null, false, volume, spatialBlend);
        public static AudioSource PlaySound(AudioClip audioClip, bool looping, float volume = 1, float spatialBlend = 0) => PlaySound("Essentials.NoId", audioClip, default, null, looping, volume, spatialBlend);
        public static AudioSource PlaySound(string id, AudioClip audioClip, Transform parent, float volume = 1, float spatialBlend = 0) => PlaySound(id, audioClip, default, parent, false, volume, spatialBlend);
        public static AudioSource PlaySound(string id, AudioClip audioClip, Transform parent, bool looping, float volume = 1, float spatialBlend = 0) => PlaySound(id, audioClip, default, parent, looping, volume, spatialBlend);
        public static AudioSource PlaySound(string id, AudioClip audioClip, Vector3 position) => PlaySound(id, audioClip, position, null, false);
        public static AudioSource PlaySound(string id, AudioClip audioClip, Vector3 position, float volume, float spatialBlend = 0) => PlaySound(id, audioClip, position, null, false, volume, spatialBlend);
        public static AudioSource PlaySound(string id, AudioClip audioClip, Vector3 position, Transform parent, float volume = 1, float spatialBlend = 0) => PlaySound(id, audioClip, position, parent, false, volume, spatialBlend);
        public static AudioSource PlaySound(string id, AudioClip audioClip, Vector3 position, bool looping, float volume = 1, float spatialBlend = 0) => PlaySound(id, audioClip, position, null, looping, volume, spatialBlend);
        public static AudioSource PlaySound(string id, AudioClip audioClip, float volume, float spatialBlend = 0) => PlaySound(id, audioClip, default, null, false, volume, spatialBlend);
        public static AudioSource PlaySound(string id, AudioClip audioClip, bool looping, float volume, float spatialBlend = 0) => PlaySound(id, audioClip, default, null, looping, volume, spatialBlend);
        
        public static AudioSource PlaySound(string id, AudioClip audioClip, Vector3 position = default, Transform parent = null, bool looping = false, float volume = 1, float spatialBlend = 0)
        {
            GameSound gameSound = new GameSound(id, audioClip, position, parent, looping, volume, spatialBlend);
            _gameSounds.Add(gameSound);
            
            if (!looping) StopSoundAfter(gameSound, audioClip.length, _cancellationToken);
            
            return gameSound.audioSource;
        }
        
        public static void StopSound(string id)
        {
            GameSound[] gameSounds = _gameSounds.Where(gameSound => gameSound.id == id).ToArray();
            
            foreach (GameSound gameSound in gameSounds) gameSound.Destroy();
            _gameSounds.RemoveWhere(gameSound => gameSound.id == id);
        }
        
        public static void StopAllSounds()
        {
            foreach (GameSound gameSound in _gameSounds) gameSound.Destroy();
            _gameSounds.Clear();
        }
        
        private static async void StopSoundAfter(GameSound gameSound, float seconds, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            
            if (!_gameSounds.Contains(gameSound)) return;
            
            gameSound.Destroy();
            _gameSounds.Remove(gameSound);
        }
        
        private static void OnApplicationQuit() => _cancellationTokenSource.Cancel();
    }
}