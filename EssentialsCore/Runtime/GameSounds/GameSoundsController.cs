using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Essentials.Core.GameSounds;
using UnityEngine;

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
            catch (Exception)
            {
                gameSound.DestroyImmediate();
                return;
            }

            if (!_gameSounds.Contains(gameSound)) return;
            gameSound.Destroy();
        }

        private static void OnApplicationQuit() => _cancellationTokenSource.Cancel();
    }
}