using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BugDefender.OpenGL.Engine.Helpers
{
    public static class SpriteBatchHelper
    {
        public static void FillScreen(this SpriteBatch spriteBatch, Texture2D texture, int width, int height, int alpha = 255)
        {
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), new Color(255, 255, 255, alpha));
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, float thickness = 1f, int alpha = 255)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, texture, point1, distance, angle, thickness, alpha);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point, float length, float angle, float thickness = 1f, int alpha = 255)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point, null, new Color(255, 255, 255, alpha), angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}
