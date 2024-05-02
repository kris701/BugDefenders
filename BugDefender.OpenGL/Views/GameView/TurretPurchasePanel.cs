using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.OpenGL.Controls;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using System;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;
using BugDefender.OpenGL.Helpers;

namespace BugDefender.OpenGL.Views.GameView
{
    public class TurretPurchasePanel : CollectionControl
    {
        public TurretDefinition Turret { get; set; }

        private readonly BugDefenderButtonControl _turretButton;
        public TurretPurchasePanel(BugDefenderGameWindow parent, TurretDefinition turret, ClickedHandler buy)
        {
            Width = 300;
            Height = 70;

            Turret = turret;
            var currentAnimation = parent.ResourcePackController.GetAnimation<TurretEntityDefinition>(turret.ID).OnIdle;
            var textureSet = parent.Textures.GetTextureSet(currentAnimation);
            Children.Add(new TileControl()
            {
                X = 0,
                Y = 0,
                Width = Width,
                Height = Height,
                FillColor = parent.Textures.GetTexture(new Guid("5c993d08-bc73-433c-a09b-0b944ac2f425")),
            });
            Children.Add(new TileControl()
            {
                X = 15,
                Y = 15,
                Width = 40,
                Height = 40,
                FillColor = textureSet.GetLoadedContent()[0]
            });
            Children.Add(new LabelControl()
            {
                X = 70,
                Y = 20,
                Width = Width - 70,
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
                Text = turret.Name
            });
            _turretButton = new BugDefenderButtonControl(parent, buy)
            {
                X = 70,
                Y = 30,
                Width = Width - 80,
                Height = 30,
                IsEnabled = false,
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
                Text = $"Unlocked at wave {turret.AvailableAtWave}",
                FillColor = parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = parent.Textures.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Tag = turret
            };
            Children.Add(_turretButton);
        }

        public void SetPurchasability(bool canBuy, bool isLocked)
        {
            if (isLocked)
                return;
            else
                _turretButton.Text = $"[{Turret.Cost}$]";
            _turretButton.IsEnabled = canBuy;
        }
    }
}
