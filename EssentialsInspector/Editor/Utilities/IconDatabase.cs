using System.IO;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector.Utilities
{
    public static class IconDatabase
    {
        public static Texture GetFolderIcon()
        {
            return EditorGUIUtility.isProSkin
                ? EditorGUIUtility.IconContent("d_Project").image
                : EditorGUIUtility.IconContent("Project").image;
        }

        public static Texture GetSettingsIcon()
        {
            return EditorGUIUtility.isProSkin
                ? EditorGUIUtility.IconContent("d_Settings").image
                : EditorGUIUtility.IconContent("Settings").image;
        }

        public static Texture GetIcon(string name)
        {
            string iconPath = "Packages/com.notrewd.essentials/EssentialsCore/Icons";
            string iconName = EditorGUIUtility.isProSkin ? "d_" + name + ".png" : name + ".png";

            string fullPath = Path.Combine(iconPath, iconName);

            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"Icon {name} not found at {fullPath}");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
        }
    }
}
