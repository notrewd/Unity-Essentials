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

            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);

            return icon;
        }

        public static Texture2D ResizeIcon(Texture2D icon, int width, int height)
        {
            Texture2D resizedIcon = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int originalX = Mathf.RoundToInt(x * (float)icon.width / width);
                    int originalY = Mathf.RoundToInt(y * (float)icon.height / height);

                    Color pixelColor = icon.GetPixel(originalX, originalY);

                    resizedIcon.SetPixel(x, y, pixelColor);
                }
            }

            resizedIcon.Apply();

            return resizedIcon;
        }
    }
}
