using UnityEngine;

namespace Essentials.Core
{
    public static class EssentialsUtility
    {
        public static Texture2D SingleTexture2DColor(Color color)
        {
            Texture2D newTexture = new Texture2D(1, 1);
            newTexture.SetPixel(0, 0, color);
            newTexture.Apply();

            return newTexture;
        }
    }
}