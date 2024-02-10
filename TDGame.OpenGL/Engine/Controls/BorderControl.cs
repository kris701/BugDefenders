﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Engine.Controls
{
    public class BorderControl : TileControl
    {
        public Texture2D BorderBrush { get; set; } = BasicTextures.GetBasicRectange(Color.Black);
        public float Thickness { get; set; } = 2;
        public IControl Child { get; set; }

        public BorderControl(IScreen parent) : base(parent)
        {
        }

        public override void Initialize()
        {
            X = Child.X;
            Y = Child.Y;
            Width = Child.Width; 
            Height = Child.Height;
            Child.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Child.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            Child.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(BorderBrush, new Vector2(X, Y), new Rectangle((int)X, (int)Y, (int)Width, (int)Thickness), GetAlphaColor());
            spriteBatch.Draw(BorderBrush, new Vector2(X, Y), new Rectangle((int)X, (int)Y, (int)Thickness, (int)Height), GetAlphaColor());
            spriteBatch.Draw(BorderBrush, new Vector2(X + Width, Y), new Rectangle((int)(X + Width), (int)Y, (int)Thickness, (int)Height), GetAlphaColor());
            spriteBatch.Draw(BorderBrush, new Vector2(X, Y + Height), new Rectangle((int)X, (int)(Y + Height), (int)(Width + Thickness), (int)Thickness), GetAlphaColor());
        }
    }
}
