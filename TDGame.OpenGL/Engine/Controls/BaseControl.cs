using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGame.OpenGL.Engine.Controls
{
    public abstract class BaseControl : BaseScalable, IControl
    {
        public Alignment HorizontalAlignment { get; set; } = Alignment.None;

        public float Rotation { get; set; } = 0;
        public bool IsVisible { get; set; } = true;
        internal float _x = 0;
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = Scale(value);
            }
        }
        internal float _y = 0;
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = Scale(value);
            }
        }
        internal float _width = 0;
        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = Scale(value);
            }
        }
        internal float _height = 0;
        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = Scale(value);
            }
        }
        public int Alpha { get; set; } = 255;
        public object Tag { get; set; }

        protected BaseControl(UIEngine parent) : base(parent)
        {
        }

        internal void ReAlign()
        {
            switch (HorizontalAlignment)
            {
                case Alignment.Left: _x = 0; break;
                case Alignment.Right: _x = Parent.ScreenWidth() - Width; break;
                case Alignment.Middle: _x = Parent.ScreenWidth() / 2 - Width / 2; break;
            }
        }

        public virtual void Initialize()
        {
            ReAlign();
        }
        public virtual void Update(GameTime gameTime)
        {
        }
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        internal Color GetAlphaColor() => new Color(255, 255, 255, Alpha);

        internal void DrawTile(GameTime gameTime, SpriteBatch spriteBatch, Texture2D texture)
        {
            if (texture.Width == 1 && texture.Height == 1)
            {
                spriteBatch.Draw(
                    texture,
                    new Vector2(X + Width / 2, Y + Height / 2),
                    new Rectangle(0, 0, (int)Width, (int)Height),
                    GetAlphaColor(),
                    Rotation,
                    new Vector2(Width / 2, Height / 2),
                    1,
                    SpriteEffects.None,
                    0);
            }
            else
            {
                var xFit = Width / texture.Width;
                var yFit = Height / texture.Height;

                spriteBatch.Draw(
                    texture,
                    new Vector2(X + Width / 2, Y + Height / 2),
                    null,
                    GetAlphaColor(),
                    Rotation,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    new Vector2(xFit, yFit),
                    SpriteEffects.None,
                    0);
            }
#if CENTERPOINT
            spriteBatch.Draw(
                BasicTextures.GetBasicRectange(Color.Black),
                new Vector2(X, Y),
                new Rectangle(0, 0, 5, 5),
                GetAlphaColor(),
                0,
                new Vector2(0, 0),
                1,
                SpriteEffects.None,
                0);
#endif
#if TILEBORDER
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(X, Y), new Rectangle((int)X, (int)Y, (int)Width, 1), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(X, Y), new Rectangle((int)X, (int)Y, 1, (int)Height), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(X + Width, Y), new Rectangle((int)(X + Width), (int)Y, 1, (int)Height), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(X, Y + Height), new Rectangle((int)X, (int)(Y + Height), (int)(Width + 1), 1), GetAlphaColor());
#endif
        }
    }
}
