using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class TurretControl : ButtonControl
    {
        private TileControl baseControl;
        public TurretControl(IScreen parent, ClickedHandler clicked = null, ClickedHandler clickedModifierA = null, ClickedHandler clickedModifierB = null) : base(parent, clicked, clickedModifierA, clickedModifierB)
        {
        }

        public override void Initialize()
        {
            var baseTexture = TextureBuilder.GetTexture(new Guid("ba2a23be-8bf7-4307-9009-8ed330ac5b7d"));
            baseControl = new TileControl(Parent)
            {
                FillColor = baseTexture,
                Width = 35,
                Height = 35,
            };
            baseControl._x = _x + (Width - baseControl.Width) / 2;
            baseControl._y = _y + (Height - baseControl.Height) / 2;
            baseControl.Initialize();
            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            baseControl.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
