using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BugDefender.Core.Game.Models.Entities.Upgrades;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
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

        public UpgradePanel(UIEngine parent, ClickedHandler buy, IUpgrade upgrade, bool canUpgrade) : base(parent)
        {
            Buy = buy;
            _nameLabel = new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = $"{upgrade.Name}",
                Height = 35
            };
            _descriptionTextbox = new TextboxControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = upgrade.GetDescriptionString(),
                Height = 85
            };
            _buyUpgradeButton = new ButtonControl(Parent, clicked: buy)
            {
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
            _nameLabel._x = _x;
            _nameLabel._y = _y;
            _nameLabel._width = _width;
            _descriptionTextbox._x = _x;
            _descriptionTextbox._y = _y + Scale(35);
            _descriptionTextbox._width = _width;
            _buyUpgradeButton._x = _x;
            _buyUpgradeButton._y = _y + Scale(35 + 85);
            _buyUpgradeButton._width = _width;

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
