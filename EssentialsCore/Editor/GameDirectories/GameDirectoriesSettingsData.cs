using System;
using UnityEngine;

namespace Essentials.Internal.GameDirectories
{
    [Serializable]
    public class GameDirectoriesSettingsData
    {
        public string className = "DirectoriesList";
        public string classLocation = "Assets";

        [TextArea]
        public string gameDirectoriesData = "{}";
    }

    [Serializable]
    public class GameDirectoryData
    {
        public string path;
        public string reference;

        /// <summary>
        /// Initializes a new instance of the GameDirectoryData class.
        /// </summary>
        /// <param name="path">The path of the game directory.</param>
        /// <param name="reference">The reference name used in the generated class.</param>
        public GameDirectoryData(string path, string reference)
        {
            this.path = path;
            this.reference = reference;
        }
    }
}