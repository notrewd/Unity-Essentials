using UnityEditor;

namespace Essentials.Internal.GameDirectories
{
    public static class GameDirectoriesSettings
    {
        public static string GetClassName() => EditorPrefs.GetString("Essentials.GameDirectoriesSettings.ClassName", "GameDirectories");
        public static string GetClassLocation() => EditorPrefs.GetString("Essentials.GameDirectoriesSettings.ClassLocation", "Assets/GameDirectories");
        public static string GetGameDirectories() => EditorPrefs.GetString("Essentials.GameDirectoriesSettings.GameDirectories", string.Empty);

        public static void SetClassName(string className) => EditorPrefs.SetString("Essentials.GameDirectoriesSettings.ClassName", className);
        public static void SetClassLocation(string classLocation) => EditorPrefs.SetString("Essentials.GameDirectoriesSettings.ClassLocation", classLocation);
        public static void SetGameDirectories(string gameDirectories) => EditorPrefs.SetString("Essentials.GameDirectoriesSettings.GameDirectories", gameDirectories);
    }
}