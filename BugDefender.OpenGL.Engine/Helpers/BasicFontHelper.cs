using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Helpers
{
    public static class BasicFonts
    {
        private static ContentManager? _content;
        private static string _contentDir = "";
        private static Dictionary<int, SpriteFont> _cache = new Dictionary<int, SpriteFont>();

        public static void Initialize(ContentManager content, string contentDir)
        {
            _contentDir = contentDir;
            _content = content;
            _cache = new Dictionary<int, SpriteFont>();
        }

        public static SpriteFont GetFont(int fontSize)
        {
            if (_cache.ContainsKey(fontSize))
                return _cache[fontSize];

            if (_content == null)
                throw new NullReferenceException("Content Manager was null!");

            var font = _content.Load<SpriteFont>($"{_contentDir}{fontSize}");
            _cache.Add(fontSize, font);
            return font;
        }
    }
}
