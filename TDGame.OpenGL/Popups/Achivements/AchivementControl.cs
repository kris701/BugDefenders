using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Popups.Achivements
{
    public class AchivementControl : TileControl
    {
        public AchivementDefinition Achivement { get; set; }

        private TileControl _iconTile;
        private TextboxControl _descriptionTextbox;

        public AchivementControl(IScreen parent, AchivementDefinition achivement) : base(parent)
        {
            Achivement = achivement;
            Width = 300;
            Height = 120;
        }

        public override void Initialize()
        {
            base.Initialize();
            _iconTile = new TileControl(Parent)
            {
                Width = 100,
                Height = 100,
                FillColor = TextureManager.GetTexture(Achivement.ID)
            };
            _iconTile._x = _x + Parent.Scale(10);
            _iconTile._y = _y + Parent.Scale(10);
            _iconTile.Initialize();

            _descriptionTextbox = new TextboxControl(Parent)
            {
                Width = 180,
                Height = 100,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                FillColor = BasicTextures.GetBasicRectange(Color.DarkCyan),
                Text = $"Achivement unlocked!{Environment.NewLine}{Achivement.Name}{Environment.NewLine}{Achivement.Description}"
            };
            _descriptionTextbox._x = _x + Parent.Scale(10) + Parent.Scale(100);
            _descriptionTextbox._y = _y + Parent.Scale(10);
            _descriptionTextbox.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _iconTile._x = _x + Parent.Scale(10);
            _iconTile._y = _y + Parent.Scale(10);
            _descriptionTextbox._x = _x + Parent.Scale(10) + Parent.Scale(100);
            _descriptionTextbox._y = _y + Parent.Scale(10);
            _descriptionTextbox._textChanged = true;

            _descriptionTextbox.Update(gameTime);
            _iconTile.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            _iconTile.Draw(gameTime, spriteBatch);
            _descriptionTextbox.Draw(gameTime, spriteBatch);
        }
    }
}
