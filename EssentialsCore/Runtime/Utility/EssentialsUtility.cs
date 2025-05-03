using UnityEngine;

namespace Essentials.Core.Utility
{
    public static class EssentialsUtility
    {
        /// <summary>
        /// Creates a 1x1 Texture2D filled with a single specified color.
        /// </summary>
        /// <param name="color">The color to fill the texture with.</param>
        /// <returns>A new 1x1 Texture2D filled with the specified color.</returns>
        public static Texture2D SingleTexture2DColor(Color color)
        {
            Texture2D newTexture = new Texture2D(1, 1);
            newTexture.SetPixel(0, 0, color);
            newTexture.Apply();

            return newTexture;
        }
    }

    public class Wrapper<T>
    {
        public T[] items;
    }
}