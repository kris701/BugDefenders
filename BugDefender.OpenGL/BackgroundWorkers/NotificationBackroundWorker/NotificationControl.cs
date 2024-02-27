using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public class NotificationControl : TileControl
    {
        public NotificationItem Item { get; set; }

        private readonly TileControl _iconTile;
        private readonly TextboxControl _descriptionTextbox;

        public NotificationControl(UIEngine parent, NotificationItem item) : base(parent)
        {
            Item = item;
            Width = 300;
            Height = 120;
            _iconTile = new TileControl(Parent);
            _descriptionTextbox = new TextboxControl(Parent);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Item.HasImage)
            {
                _iconTile.Width = 75;
                _iconTile.Height = 75;
                _iconTile.FillColor = Parent.UIResources.GetTexture(Item.Definition.ID);
                _iconTile._x = _x + Scale(20);
                _iconTile._y = _y + Scale(20);
                _iconTile.Initialize();
            }

            _descriptionTextbox.Width = 200;
            _descriptionTextbox.Height = 100;
            _descriptionTextbox.Font = BasicFonts.GetFont(10);
            _descriptionTextbox.FontColor = Color.White;
            _descriptionTextbox.Text = $"{Item.PreFix}{Environment.NewLine}{Item.Definition.Name}{Environment.NewLine}{Item.Definition.Description}";
            if (!Item.HasImage)
            {
                _descriptionTextbox._width = 260;
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
