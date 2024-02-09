using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TDGame.OpenGL.Engine.Controls
{
    public abstract class BaseControl : IControl
    {
        public event UIEvent? UIChanged;
        public string Name { get; set; } = "";
        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    if (UIChanged != null)
                        UIChanged.Invoke();
                }
            }
        }
        public bool IsEnabled { get; set; } = true;
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 0;
        public virtual int Height { get; set; } = 0;
        public int Row { get; set; } = 0;
        public int Column { get; set; } = 0;
        public int RowSpan { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;

        protected BaseControl()
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent(ContentManager content)
        {

        }

        public virtual void Refresh()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public void UpdateUI()
        {
            if (IsEnabled)
            {
                if (UIChanged != null)
                    UIChanged.Invoke();
                else
                    Refresh();
            }
        }

        public virtual List<IControl> GetLookupStack()
        {
            return new List<IControl>() { this };
        }
    }
}
