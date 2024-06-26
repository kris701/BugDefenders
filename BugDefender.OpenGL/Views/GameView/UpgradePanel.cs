﻿using BugDefender.Core.Game.Models.Entities.Upgrades;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using System;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class UpgradePanel : CollectionControl
    {
        public UpgradeDefinition? Upgrade { get; set; }

        private readonly LabelControl _nameLabel;
        private readonly TextboxControl _descriptionTextbox;
        private readonly BugDefenderButtonControl _buyUpgradeButton;

        public UpgradePanel(BugDefenderGameWindow parent, ClickedHandler buy)
        {
            Width = 300;
            Height = 160;
            IsVisible = false;
            Children.Add(new TileControl()
            {
                Width = Width,
                Height = Height,
                FillColor = parent.Textures.GetTexture(new Guid("8799e365-3b1c-47fa-b11b-83173f6d4bca")),
            });
            _nameLabel = new LabelControl()
            {
                Y = 10,
                Height = 25,
                Width = Width,
                Font = parent.Fonts.GetFont(FontSizes.Ptx12),
                FontColor = Color.White,
            };
            Children.Add(_nameLabel);
            _descriptionTextbox = new TextboxControl()
            {
                Y = 25,
                Width = Width,
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                Height = 85,
                Margin = 15
            };
            Children.Add(_descriptionTextbox);
            _buyUpgradeButton = new BugDefenderButtonControl(parent, clicked: buy)
            {
                X = 10,
                Y = 115,
                Width = Width - 20,
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                FillColor = BasicTextures.GetBasicRectange(Color.Transparent),
                FillClickedColor = parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent),
                Height = 35
            };
            Children.Add(_buyUpgradeButton);
        }

        public void SetPurchasability(bool canUpgrade, bool isLocked)
        {
            if (isLocked)
            {
                _nameLabel.Text = "???";
                _descriptionTextbox.Text = "???";
                _buyUpgradeButton.Text = "???";
            }
            else if (Upgrade != null)
            {
                _nameLabel.Text = $"{Upgrade.Name}";
                _descriptionTextbox.Text = Upgrade.ToString();
                _buyUpgradeButton.Text = $"[{Upgrade.Cost}$] Buy";
            }

            _buyUpgradeButton.IsEnabled = canUpgrade;
            if (canUpgrade)
                _buyUpgradeButton.FontColor = Color.White;
            else
                _buyUpgradeButton.FontColor = Color.Gray;
        }

        public void SetUpgrade(UpgradeDefinition upgrade, bool canUpgrade, bool isLocked)
        {
            _buyUpgradeButton.Tag = upgrade;
            Upgrade = upgrade;
            SetPurchasability(canUpgrade, isLocked);
        }
    }
}
