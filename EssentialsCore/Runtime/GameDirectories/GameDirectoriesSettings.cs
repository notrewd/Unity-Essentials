using System.IO;
using System.Linq;
using System.Text;
using Essentials.Core.GameDirectories;
using UnityEditor;
using UnityEngine;

namespace Essentials.Internal.GameDirectories
{
    public static class GameDirectoriesSettings
    {
        public static string GetClassName() => EditorPrefs.GetString("Essentials.GameDirectoriesSettings.ClassName", "DirectoriesList");
        public static string GetClassLocation() => EditorPrefs.GetString("Essentials.GameDirectoriesSettings.ClassLocation", "Assets");
        public static string GetGameDirectories() => EditorPrefs.GetString("Essentials.GameDirectoriesSettings.GameDirectories", string.Empty);

        public static void SetClassName(string className) => EditorPrefs.SetString("Essentials.GameDirectoriesSettings.ClassName", className);
        public static void SetClassLocation(string classLocation) => EditorPrefs.SetString("Essentials.GameDirectoriesSettings.ClassLocation", classLocation);
        public static void SetGameDirectories(string gameDirectories) => EditorPrefs.SetString("Essentials.GameDirectoriesSettings.GameDirectories", gameDirectories);

        public static void GenerateClass(GameDirectory[] directories)
        {
            string className = GetClassName();
            string classLocation = GetClassLocation();

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("namespace Essentials.Core.GameDirectories");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"\tpublic static class {className}");
            stringBuilder.AppendLine("\t{");

            foreach (GameDirectory directory in directories)
            {
                if (string.IsNullOrEmpty(directory.reference)) continue;
                stringBuilder.AppendLine($"\t\tpublic static string {directory.reference}");
                stringBuilder.AppendLine("\t\t{");
                stringBuilder.AppendLine($"string path = Application.persistentDataPath + Path.DirectorySeparatorChar + {string.Join(" + Path.DirectorySeparatorChar + ", directory.path.Split("/").Select(x => $"\"{x}\""))};");
                stringBuilder.AppendLine("\t\t\tif (!Directory.Exists(path)) Directory.CreateDirectory(path);");
                stringBuilder.AppendLine("\t\t\treturn path;");
                stringBuilder.AppendLine("\t\t}");
            }

            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");

            string classPath = $"{classLocation}/{className}.cs";

            if (AssetDatabase.LoadAssetAtPath<MonoScript>(classPath) != null) AssetDatabase.DeleteAsset(classPath);

            StreamWriter streamWriter = new StreamWriter(Application.dataPath + classPath[6..]);
            streamWriter.Write(stringBuilder.ToString());
            streamWriter.Close();

            AssetDatabase.Refresh();

            
        }
    }
}