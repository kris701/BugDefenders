using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.BackgroundWorkers.AchivementBackroundWorker
{
    public class NotificationControl : TileControl
    {
        public NotificationItem Item { get; set; }

        private TileControl _iconTile;
        private TextboxControl _descriptionTextbox;

        public NotificationControl(UIEngine parent, NotificationItem item) : base(parent)
        {
            Item = item;
            Width = 300;
            Height = 120;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Item.HasImage)
            {
                _iconTile = new TileControl(Parent)
                {
                    Width = 75,
                    Height = 75,
                    FillColor = TextureManager.GetTexture(Item.Definition.ID)
                };
                _iconTile._x = _x + Scale(20);
                _iconTile._y = _y + Scale(20);
                _iconTile.Initialize();
            }

            _descriptionTextbox = new TextboxControl(Parent)
            {
                Width = 200,
                Height = 100,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = $"{Item.PreFix}{Environment.NewLine}{Item.Definition.Name}{Environment.NewLine}{Item.Definition.Description}"
            };
            if (!Item.HasImage)
            {
                _descriptionTextbox._width = 280;
                _descriptionTextbox._x = _x + Scale(10);
            }
            else
                _descriptionTextbox._x = _x + Scale(10) + Scale(100);
            _descriptionTextbox._y = _y + Scale(10);
            _descriptionTextbox.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Item.HasImage)
            {
                _iconTile._x = _x + Scale(20);
                _iconTile._y = _y + Scale(20);
                _descriptionTextbox._x = _x + Scale(10) + Scale(75);
            }
            else
                _descriptionTextbox._x = _x + Scale(20);
            _descriptionTextbox._y = _y + Scale(20);
            _descriptionTextbox._textChanged = true;

            _descriptionTextbox.Update(gameTime);
            if (Item.HasImage)
                _iconTile.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            if (Item.HasImage)
                _iconTile.Draw(gameTime, spriteBatch);
            _descriptionTextbox.Draw(gameTime, spriteBatch);
        }
    }
}
