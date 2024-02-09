using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class PanelControl : BaseControl
    {
        public string FillColorName { get; set; } = "";
        public Texture2D FillColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public PanelControl()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            if (FillColorName != "")
                FillColor = content.Load<Texture2D>(FillColorName);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(FillColor, new Rectangle(X, Y, Width, Height), Color.White);
        }
    }
}
