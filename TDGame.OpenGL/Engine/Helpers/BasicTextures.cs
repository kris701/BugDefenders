using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TDGame.OpenGL.Engine.Helpers
{
    public static class BasicTextures
    {
        private static GraphicsDevice? _graphicsDevice;
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _cache = new Dictionary<Color, Texture2D>();
        }

        private static Dictionary<Color, Texture2D> _cache;
        public static Texture2D GetBasicTexture(Color target)
        {
            if (_cache.ContainsKey(target))
                return _cache[target];
            Texture2D texture = new Texture2D(_graphicsDevice, 1, 1);
            texture.SetData(new[] { target });
            _cache.Add(target, texture); 
            return texture;
        }
    }
}
