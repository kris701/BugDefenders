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

        internal void DrawTile(GameTime gameTime, SpriteBatch spriteBatch, Texture2D texture)
        {
            if (FillColor.Width == 1 && FillColor.Height == 1)
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
