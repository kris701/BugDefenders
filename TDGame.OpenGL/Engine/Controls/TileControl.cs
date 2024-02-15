using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Engine.Controls
{
    public class TileControl : BaseControl
    {
        public Texture2D FillColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public float Rotation { get; set; } = 0;

        public TileControl(IScreen parent) : base(parent)
        {
        }

        public override void Initialize()
        {
            if (Width == 0)
                Width = FillColor.Width;
            if (Height == 0)
                Height = FillColor.Height;
            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            DrawTile(gameTime, spriteBatch, FillColor);
        }
    }
}
