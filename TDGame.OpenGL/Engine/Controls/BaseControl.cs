using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Engine.Controls
{
    public abstract class BaseControl : IControl
    {
        public IScreen Parent { get; internal set; }
        public Alignment HorizontalAlignment { get; set; } = Alignment.None;

        public bool IsVisible { get; set; } = true;
        private float _x = 0;
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = Parent.Scale(value);
            }
        }
        private float _y = 0;
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = Parent.Scale(value);
            }
        }
        private float _width = 0;
        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = Parent.Scale(value);
            }
        }
        private float _height = 0;
        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = Parent.Scale(value);
            }
        }
        public int Alpha { get; set; } = 255;
        public object Tag { get; set; }

        protected BaseControl(IScreen parent)
        {
            Parent = parent;
        }

        internal void ReAlign()
        {
            switch (HorizontalAlignment)
            {
                case Alignment.Left: _x = 0; break;
                case Alignment.Right: _x = Parent.Parent.ScreenWidth() - Width; break;
                case Alignment.Middle: _x = Parent.Parent.ScreenWidth() / 2 - Width / 2; break;
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
    }
}
