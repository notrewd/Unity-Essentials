using System.IO;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector.Utilities
{
    /// <summary>
    /// Provides utility methods for retrieving and manipulating editor icons.
    /// </summary>
    public static class IconDatabase
    {
        /// <summary>
        /// Retrieves an editor icon by name, considering the current editor skin (Pro/Personal).
        /// Icons are expected to be located in "Packages/com.notrewd.essentials/EssentialsCore/Icons".
        /// Pro skin icons should be prefixed with "d_".
        /// </summary>
        /// <param name="name">The base name of the icon (e.g., "Settings@32").</param>
        /// <returns>The loaded Texture2D icon, or null if not found.</returns>
        public static Texture2D GetIcon(string name)
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

        /// <summary>
        /// Resizes a Texture2D icon to the specified dimensions using nearest-neighbor scaling.
        /// </summary>
        /// <param name="icon">The original icon Texture2D.</param>
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        /// <returns>A new Texture2D with the resized icon.</returns>
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
