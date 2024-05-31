using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Essentials.Core.GameSounds;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Essentials.Internal.GameSounds
{
    public static class GameSoundsController
    {
        private static readonly HashSet<GameSound> _gameSounds = new HashSet<GameSound>();

        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static CancellationToken _cancellationToken;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _cancellationToken = _cancellationTokenSource.Token;
            Application.quitting += OnApplicationQuit;
        }

        public static HashSet<GameSound> GetGameSounds() => _gameSounds;

        public static async void StopSoundAfter(GameSound gameSound, float seconds)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), _cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (!_gameSounds.Contains(gameSound)) return;

                Object.DestroyImmediate(gameSound.GetGameObject());
                return;
            }

            if (!_gameSounds.Contains(gameSound)) return;

            _gameSounds.Remove(gameSound);
            Object.Destroy(gameSound.GetGameObject());
        }

        public static void StopSounds(GameSound[] gameSounds)
        {
            if (gameSounds.Length == 0) return;

            List<GameSound> soundsToRemove = new List<GameSound>();

            foreach (GameSound gameSound in gameSounds)
            {
                if (!_gameSounds.Contains(gameSound)) continue;
                soundsToRemove.Add(gameSound);
            }

            foreach (GameSound gameSound in soundsToRemove)
            {
                _gameSounds.Remove(gameSound);
                Object.Destroy(gameSound.GetGameObject());
            }
        }

        public static void StopAllSounds()
        {

        }

        private static void OnApplicationQuit() => StopAllSounds();
    }
}