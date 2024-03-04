using BugDefender.Core.Game.Models.Entities.Upgrades;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class UpgradePanel : TileControl
    {
        public event ClickedHandler Buy;
        public List<IControl> Children { get; set; }
        private readonly LabelControl _nameLabel;
        private readonly TextboxControl _descriptionTextbox;
        private readonly ButtonControl _buyUpgradeButton;

        public UpgradePanel(GameWindow parent, ClickedHandler buy, UpgradeDefinition upgrade, bool canUpgrade)
        {
            Buy = buy;
            _nameLabel = new LabelControl()
            {
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                Text = $"{upgrade.Name}",
                Height = 25
            };
            _descriptionTextbox = new TextboxControl()
            {
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = upgrade.GetDescriptionString(),
                Height = 85,
                Margin = 20
            };
            _buyUpgradeButton = new ButtonControl(parent, clicked: buy)
            {
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                FillColor = BasicTextures.GetBasicRectange(Color.Transparent),
                FillClickedColor = parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent),
                Text = $"[{upgrade.Cost}$] Buy",
                Height = 35,
                Tag = upgrade
            };
            Children = new List<IControl>() {
                _nameLabel,
                _descriptionTextbox,
                _buyUpgradeButton
            };

            _buyUpgradeButton.IsEnabled = canUpgrade;

            if (!canUpgrade)
            {
                _nameLabel.FontColor = Color.Gray;
                _descriptionTextbox.FontColor = Color.Gray;
                _buyUpgradeButton.FontColor = Color.Gray;
            }
            else
            {
                _nameLabel.FontColor = Color.White;
                _descriptionTextbox.FontColor = Color.White;
                _buyUpgradeButton.FontColor = Color.White;
            }
        }

        public void SetVisibility(bool visible)
        {
            IsVisible = visible;
            foreach (var child in Children)
                child.IsVisible = visible;
        }

        public override void Initialize()
        {
            _nameLabel.X = X;
            _nameLabel.Y = Y + 10;
            _nameLabel.Width = Width;
            _descriptionTextbox.X = X;
            _descriptionTextbox.Y = Y + 25;
            _descriptionTextbox.Width = Width;
            _buyUpgradeButton.X = X + 10;
            _buyUpgradeButton.Y = Y + 30 + 85;
            _buyUpgradeButton.Width = Width - 20;

            foreach (var child in Children)
                child.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var child in Children)
                child.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            foreach (var child in Children)
                child.Draw(gameTime, spriteBatch);
        }
    }
}
