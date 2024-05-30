using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Essentials.Internal.GameSounds
{
    [Serializable]
    public class GameSoundsData : ScriptableObject
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