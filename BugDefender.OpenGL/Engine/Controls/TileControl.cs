﻿using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class TileControl : BaseControl
    {
        public Texture2D FillColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);

        public TileControl(UIEngine parent) : base(parent)
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