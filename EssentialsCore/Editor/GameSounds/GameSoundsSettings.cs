using System.IO;
using UnityEditor;
using UnityEngine;

namespace Essentials.Internal.GameSounds
{
    /// <summary>
    /// Provides static methods for managing GameSoundsData assets.
    /// </summary>
    public static class GameSoundsSettings
    {
        /// <summary>
        /// Gets the GameSoundsData asset, creating it if it doesn't exist.
        /// </summary>
        /// <returns>The GameSoundsData asset.</returns>
        public static GameSoundsData GetData()
        {
            if (File.Exists(Path.Combine(Application.dataPath, "EssentialsData", "Resources", "GameSoundsData.asset")))
            {
                return AssetDatabase.LoadAssetAtPath<GameSoundsData>(Path.Combine("Assets", "EssentialsData", "Resources", "GameSoundsData.asset"));
            }
            else
            {
                if (!Directory.Exists(Path.Combine(Application.dataPath, "EssentialsData", "Resources"))) Directory.CreateDirectory(Path.Combine(Application.dataPath, "EssentialsData", "Resources"));

                GameSoundsData gameSoundsData = ScriptableObject.CreateInstance<GameSoundsData>();

                AssetDatabase.CreateAsset(gameSoundsData, Path.Combine("Assets", "EssentialsData", "Resources", "GameSoundsData.asset"));
                AssetDatabase.SaveAssets();

                return gameSoundsData;
            }
        }

        /// <summary>
        /// Deletes the GameSoundsData asset if it exists.
        /// </summary>
        public static void ResetData()
        {
            if (!File.Exists(Path.Combine(Application.dataPath, "EssentialsData", "Resources", "GameSoundsData.asset"))) return;

            AssetDatabase.DeleteAsset(Path.Combine("Assets", "EssentialsData", "Resources", "GameSoundsData.asset"));
            AssetDatabase.Refresh();
        }
    }
}