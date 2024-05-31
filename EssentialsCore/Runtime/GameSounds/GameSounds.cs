using System.Linq;
using Essentials.Internal.GameSounds;
using UnityEngine;

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
}