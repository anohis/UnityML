using UnityEngine;

namespace Utility
{
    public static class TextureExtension
    {
        public static void SetPixel(this Texture2D texture, Vector2Int pixel, Color color)
        {
            texture.SetPixel(pixel.x, pixel.y, color);
        }

        public static void Fill(this Texture2D texture, Color color)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }
}