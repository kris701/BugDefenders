using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGame.OpenGL.Engine.Helpers
{
    public static class SpriteBatchHelper
    {
        public static void FillScreen(this SpriteBatch spriteBatch, Texture2D texture, int width, int height, int alpha = 255)
        {
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), new Color(255, 255, 255, alpha));
        }
    }
}
