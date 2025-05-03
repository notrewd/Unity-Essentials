using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Essentials.Core.GameSounds;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Essentials.Internal.GameSounds
{
    /// <summary>
    /// Internal controller responsible for managing the lifecycle and state of GameSound instances.
    /// </summary>
    public static class GameSoundsController
    {
        private static readonly HashSet<GameSound> _gameSounds = new HashSet<GameSound>();

        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes the GameSoundsController, setting up the cancellation token and subscribing to the application quitting event.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _cancellationToken = _cancellationTokenSource.Token;
            Application.quitting += OnApplicationQuit;
        }

        /// <summary>
        /// Gets the set of currently active GameSound instances.
        /// </summary>
        /// <returns>A HashSet containing the active GameSound instances.</returns>
        public static HashSet<GameSound> GetGameSounds() => _gameSounds;

        /// <summary>
        /// Stops a specific GameSound instance after a specified delay.
        /// Handles cancellation if the application quits before the delay completes.
        /// </summary>
        /// <param name="gameSound">The GameSound instance to stop.</param>
        /// <param name="seconds">The delay in seconds before stopping the sound.</param>
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

        /// <summary>
        /// Stops an array of GameSound instances immediately.
        /// Removes them from the active list and destroys their associated GameObjects.
        /// </summary>
        /// <param name="gameSounds">An array of GameSound instances to stop.</param>
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

        /// <summary>
        /// Stops all currently active GameSound instances immediately.
        /// Clears the active list and destroys all associated GameObjects.
        /// </summary>
        public static void StopAllSounds()
        {
            foreach (GameSound gameSound in _gameSounds) Object.Destroy(gameSound.GetGameObject());
            _gameSounds.Clear();
        }

        /// <summary>
        /// Callback method executed when the application is quitting. Ensures all sounds are stopped.
        /// </summary>
        private static void OnApplicationQuit() => StopAllSounds();
    }
}