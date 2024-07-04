using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Essentials.Internal.GameSounds
{
    [Serializable]
    public class GameSoundsData : ScriptableObject
    {
        public GameSoundSettings defaultSettings;
        public List<GameSoundGroup> gameSoundGroups = new List<GameSoundGroup>();
    }

    [Serializable]
    public class GameSoundGroup
    {
        public string name = "New Group";
        public GameSoundSettings settings = new GameSoundSettings();
    }

    [Serializable]
    public class GameSoundSettings
    {
        public float volume = 1f;
        public bool loop = false;
        public int priority = 128;
        public float spatialBlend = 1f;
        public bool spatialize = true;
        public float dopplerLevel = 1f;
        public float minDistance = 1f;
        public float maxDistance = 500f;
        public float panStereo = 0f;
        public float reverbZoneMix = 0f;
        public AudioMixerGroup audioMixerGroup = null;
    }
}