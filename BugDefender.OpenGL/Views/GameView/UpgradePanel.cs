using BugDefender.Core.Game.Models.Entities.Upgrades;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using System;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class UpgradePanel : CollectionControl
    {
        public UpgradeDefinition? Upgrade { get; set; }

        private readonly LabelControl _nameLabel;
        private readonly LabelControl _questionLabel;
        private readonly TextboxControl _descriptionTextbox;
        private readonly ButtonControl _buyUpgradeButton;

        public UpgradePanel(BugDefenderGameWindow parent, ClickedHandler buy)
        {
            Width = 300;
            Height = 160;
            IsVisible = false;
            Children.Add(new TileControl()
            {
                Width = Width,
                Height = Height,
                FillColor = parent.TextureController.GetTexture(new Guid("8799e365-3b1c-47fa-b11b-83173f6d4bca")),
            });
            _questionLabel = new LabelControl()
            {
                Width = Width,
                Height = Height,
                Font = BasicFonts.GetFont(24),
                Text = "???",
                IsVisible = false
            };
            Children.Add(_questionLabel);
            _nameLabel = new LabelControl()
            {
                Y = 10,
                Height = 25,
                Width = Width,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
            };
            Children.Add(_nameLabel);
            _descriptionTextbox = new TextboxControl()
            {
                Y = 25,
                Width = Width,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Height = 85,
                Margin = 20
            };
            Children.Add(_descriptionTextbox);
            _buyUpgradeButton = new ButtonControl(parent, clicked: buy)
            {
                X = 10,
                Y = 115,
                Width = Width - 20,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                FillColor = BasicTextures.GetBasicRectange(Color.Transparent),
                FillClickedColor = parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent),
                Height = 35
            };
            Children.Add(_buyUpgradeButton);
        }

        public void SetPurchasability(bool canUpgrade, bool isLocked)
        {
            _questionLabel.IsVisible = isLocked;
            _nameLabel.IsVisible = !isLocked;
            _descriptionTextbox.IsVisible = !isLocked;
            _buyUpgradeButton.IsVisible = !isLocked;
            _buyUpgradeButton.IsEnabled = canUpgrade;
            if (canUpgrade)
                _buyUpgradeButton.FontColor = Color.White;
            else
                _buyUpgradeButton.FontColor = Color.Gray;
        }

        public void SetUpgrade(UpgradeDefinition upgrade, bool canUpgrade, bool isLocked)
        {
            _nameLabel.Text = $"{upgrade.Name}";
            _descriptionTextbox.Text = upgrade.ToString();
            _buyUpgradeButton.Text = $"[{upgrade.Cost}$] Buy";
            _buyUpgradeButton.Tag = upgrade;
            Upgrade = upgrade;
            SetPurchasability(canUpgrade, isLocked);
        }
    }
}
