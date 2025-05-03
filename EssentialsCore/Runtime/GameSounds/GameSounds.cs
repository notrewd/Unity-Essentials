using System.Linq;
using Essentials.Internal.GameSounds;
using UnityEngine;

namespace Essentials.Core.GameSounds
{
    /// <summary>
    /// Provides static methods for creating, playing, and managing GameSound instances.
    /// </summary>
    public static class GameSounds
    {
        /// <summary>
        /// Creates a new GameSound instance with the specified audio clip.
        /// </summary>
        /// <param name="audioClip">The audio clip to use.</param>
        /// <returns>A new GameSound instance.</returns>
        public static GameSound CreateSound(AudioClip audioClip) => new(audioClip);

        /// <summary>
        /// Creates and immediately plays a new GameSound instance with the specified audio clip.
        /// </summary>
        /// <param name="audioClip">The audio clip to play.</param>
        public static void PlaySound(AudioClip audioClip) => CreateSound(audioClip).Play();

        /// <summary>
        /// Creates, sets the ID, and immediately plays a new GameSound instance with the specified audio clip.
        /// </summary>
        /// <param name="id">The identifier to assign to the GameSound.</param>
        /// <param name="audioClip">The audio clip to play.</param>
        public static void PlaySound(string id, AudioClip audioClip) => CreateSound(audioClip).SetId(id).Play();

        /// <summary>
        /// Stops all currently playing GameSound instances with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the GameSounds to stop.</param>
        public static void StopSound(string id)
        {
            GameSoundsController.StopSounds(GameSoundsController.GetGameSounds().Where(gameSound => gameSound.GetId() == id).ToArray());
        }

        /// <summary>
        /// Stops all currently playing GameSound instances managed by the GameSounds system.
        /// </summary>
        public static void StopAllSounds() => GameSoundsController.StopAllSounds();

        /// <summary>
        /// Checks if any GameSound instance with the specified identifier is currently playing.
        /// </summary>
        /// <param name="id">The identifier to check.</param>
        /// <returns>True if a GameSound with the specified ID is playing, false otherwise.</returns>
        public static bool IsPlaying(string id)
        {
            foreach (GameSound gameSound in GameSoundsController.GetGameSounds())
            {
                if (gameSound.GetId() == id) return true;
            }

            return false;
        }
    }
}