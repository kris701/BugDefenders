using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public class NotificationControl : BugDefenderButtonControl
    {
        public NotificationItem Item { get; set; }

        private readonly TileControl _iconTile;
        private readonly TextboxControl _descriptionTextbox;

        public NotificationControl(BugDefenderGameWindow parent, NotificationItem item) : base(parent)
        {
            Item = item;
            Width = 300;
            Height = 120;
            _iconTile = new TileControl();
            _descriptionTextbox = new TextboxControl();
            Clicked += item.Clicked;
            FillClickedColor = BasicTextures.GetBasicRectange(Color.Transparent);
            FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Item.HasImage)
            {
                _iconTile.Width = 75;
                _iconTile.Height = 75;
                _iconTile.FillColor = Parent.TextureController.GetTexture(Item.Definition.ID);
                _iconTile.X = X + 20;
                _iconTile.Y = Y + 20;
                _iconTile.Initialize();
            }

            _descriptionTextbox.Width = 200;
            _descriptionTextbox.Height = 100;
            _descriptionTextbox.Font = BasicFonts.GetFont(10);
            _descriptionTextbox.FontColor = Color.White;
            _descriptionTextbox.Text = $"{Item.PreFix}{Environment.NewLine}{Item.Definition.Name}{Environment.NewLine}{Item.Definition.Description}";
            if (!Item.HasImage)
            {
                _descriptionTextbox.Width = 260;
                _descriptionTextbox.X = X + 10;
            }
            else
                _descriptionTextbox.X = X + 10 + 100;
            _descriptionTextbox.Y = Y + 10;
            _descriptionTextbox.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Item.HasImage)
            {
                _iconTile.X = X + 20;
                _iconTile.Y = Y + 20;
                _descriptionTextbox.X = X + 10 + 75;
            }
            else
                _descriptionTextbox.X = X + 20;
            _descriptionTextbox.Y = Y + 20;
            _descriptionTextbox.Text = _descriptionTextbox.Text;

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
