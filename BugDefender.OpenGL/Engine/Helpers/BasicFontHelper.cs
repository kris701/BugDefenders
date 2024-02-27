using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TDGame.OpenGL.Engine.Helpers
{
    public static class BasicFonts
    {
        private static ContentManager _content;
        private static Dictionary<int, SpriteFont> _cache = new Dictionary<int, SpriteFont>();

        public static void Initialize(ContentManager content)
        {
            _content = content;
            _cache = new Dictionary<int, SpriteFont>();
        }

        public static SpriteFont GetFont(int fontSize)
        {
            if (_cache.ContainsKey(fontSize))
                return _cache[fontSize];

            var font = _content.Load<SpriteFont>($"DefaultFonts/DefaultFont{fontSize}");
            _cache.Add(fontSize, font);
            return font;
        }
    }
}
