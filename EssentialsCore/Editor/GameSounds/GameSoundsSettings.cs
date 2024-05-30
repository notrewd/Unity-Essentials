using System.IO;
using UnityEditor;
using UnityEngine;

namespace Essentials.Internal.GameSounds
{
    public static class GameSoundsSettings
    {
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

        public static void ResetData()
        {
            if (!File.Exists(Path.Combine(Application.dataPath, "EssentialsData", "Resources", "GameSoundsData.asset"))) return;

            AssetDatabase.DeleteAsset(Path.Combine("Assets", "EssentialsData", "Resources", "GameSoundsData.asset"));
            AssetDatabase.Refresh();
        }
    }
}