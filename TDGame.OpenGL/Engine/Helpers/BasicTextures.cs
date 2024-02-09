using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGame.OpenGL.Engine.Helpers
{
    public static class BasicTextures
    {
        private static GraphicsDevice? _graphicsDevice;
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        private static Texture2D? _transparentTexture;
        public static Texture2D Transparent
        {
            get
            {
                if (_transparentTexture != null)
                    return _transparentTexture;

                Texture2D texture = new Texture2D(_graphicsDevice, 1, 1);
                texture.SetData(new[] { Color.Transparent });
                _transparentTexture = texture;
                return texture;
            }
        }

        private static Texture2D? _blackTexture;
        public static Texture2D Black
        {
            get
            {
                if (_blackTexture != null)
                    return _blackTexture;

                Texture2D texture = new Texture2D(_graphicsDevice, 1, 1);
                texture.SetData(new[] { Color.Black });
                _blackTexture = texture;
                return texture;
            }
        }

        private static Texture2D? _whiteTexture;
        public static Texture2D White
        {
            get
            {
                if (_whiteTexture != null)
                    return _whiteTexture;

                Texture2D texture = new Texture2D(_graphicsDevice, 1, 1);
                texture.SetData(new[] { Color.White });
                _whiteTexture = texture;
                return texture;
            }
        }

        private static Texture2D? _grayTexture;
        public static Texture2D Gray
        {
            get
            {
                if (_grayTexture != null)
                    return _grayTexture;

                Texture2D texture = new Texture2D(_graphicsDevice, 1, 1);
                texture.SetData(new[] { Color.Gray });
                _grayTexture = texture;
                return texture;
            }
        }
    }
}
