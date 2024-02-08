using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Helpers
{
    public static class BasicFonts
    {
        private static ContentManager _content;

        public static void Initialize(ContentManager content)
        {
            _content = content;
        }

        private static SpriteFont? _font8pt;
        public static SpriteFont Font8pt
        {
            get
            {
                if (_font8pt != null)
                    return _font8pt;

                _font8pt = _content.Load<SpriteFont>("DefaultFonts/DefaultFont8");
                return _font8pt;
            }
        }

        private static SpriteFont? _font10pt;
        public static SpriteFont Font10pt
        {
            get
            {
                if (_font10pt != null)
                    return _font10pt;

                _font10pt = _content.Load<SpriteFont>("DefaultFonts/DefaultFont10");
                return _font10pt;
            }
        }

        private static SpriteFont? _font12pt;
        public static SpriteFont Font12pt
        {
            get
            {
                if (_font12pt != null)
                    return _font12pt;

                _font12pt = _content.Load<SpriteFont>("DefaultFonts/DefaultFont12");
                return _font12pt;
            }
        }

        private static SpriteFont? _font16pt;
        public static SpriteFont Font16pt
        {
            get
            {
                if (_font16pt != null)
                    return _font16pt;

                _font16pt = _content.Load<SpriteFont>("DefaultFonts/DefaultFont16");
                return _font16pt;
            }
        }

        private static SpriteFont? _font24pt;
        public static SpriteFont Font24pt
        {
            get
            {
                if (_font24pt != null)
                    return _font24pt;

                _font24pt = _content.Load<SpriteFont>("DefaultFonts/DefaultFont24");
                return _font24pt;
            }
        }

        private static SpriteFont? _font48pt;
        public static SpriteFont Font48pt
        {
            get
            {
                if (_font48pt != null)
                    return _font48pt;

                _font48pt = _content.Load<SpriteFont>("DefaultFonts/DefaultFont48");
                return _font48pt;
            }
        }

        private static SpriteFont? _font72pt;
        public static SpriteFont Font72pt
        {
            get
            {
                if (_font72pt != null)
                    return _font72pt;

                _font72pt = _content.Load<SpriteFont>("DefaultFonts/DefaultFont72");
                return _font72pt;
            }
        }
    }
}
