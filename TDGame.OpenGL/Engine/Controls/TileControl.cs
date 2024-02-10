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
        public bool ForceFit { get; set; } = false;

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

            if (FillColor.Width == 1 && FillColor.Height == 1)
            {
                spriteBatch.Draw(
                    FillColor,
                    new Vector2(X, Y),
                    new Rectangle(0, 0, (int)Width, (int)Height),
                    GetAlphaColor(),
                    0,
                    new Vector2(),
                    1,
                    SpriteEffects.None,
                    0);
            }
            else
            {
                if (ForceFit)
                {
                    var xFit = Width / FillColor.Width;
                    var yFit = Height / FillColor.Height;

                    spriteBatch.Draw(
                        FillColor,
                        new Vector2(X, Y),
                        null,
                        GetAlphaColor(),
                        0,
                        new Vector2(),
                        new Vector2(xFit, yFit),
                        SpriteEffects.None,
                        0);
                }
                else
                    spriteBatch.Draw(
                        FillColor,
                        new Vector2(X, Y),
                        null,
                        GetAlphaColor(),
                        0,
                        new Vector2(),
                        Parent.ScaleValue,
                        SpriteEffects.None,
                        0);
            }
        }
    }
}
