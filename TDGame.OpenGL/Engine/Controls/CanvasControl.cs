using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class CanvasControl : BaseChildrenContainer
    {
        public string FillColorName { get; set; } = "";
        public Texture2D FillColor { get; set; } = BasicTextures.GetBasicTexture(Color.Transparent);

        public CanvasControl()
        {
        }

        public override void Initialize()
        {
            foreach (var child in Children)
            {
                child.UIChanged += () =>
                {
                    Height = 0;
                    UpdateUI();
                };
                child.Initialize();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(FillColor, new Rectangle(X, Y, Width, Height), Color.White);
            foreach (var child in Children)
                child.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent(ContentManager content)
        {
            if (FillColorName != "")
                FillColor = content.Load<Texture2D>(FillColorName);

            foreach (var child in Children)
                child.LoadContent(content);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            foreach (var child in Children)
                child.Refresh();
        }
    }
}
