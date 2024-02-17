using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class TurretControl : AnimatedButtonControl
    {
        private TileControl baseControl;
        private LabelControl turretLevelControl;
        public TurretControl(IScreen parent, ClickedHandler clicked = null, ClickedHandler clickedModifierA = null, ClickedHandler clickedModifierB = null) : base(parent, clicked, clickedModifierA, clickedModifierB)
        {
        }

        public override void Initialize()
        {
            var baseTexture = TextureManager.GetTexture(new Guid("ba2a23be-8bf7-4307-9009-8ed330ac5b7d"));
            baseControl = new TileControl(Parent)
            {
                FillColor = baseTexture,
                Width = 35,
                Height = 35,
            };
            baseControl._x = _x + (Width - baseControl.Width) / 2;
            baseControl._y = _y + (Height - baseControl.Height) / 2;
            baseControl.Initialize();

            turretLevelControl = new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                IsVisible = false,
                FontColor = Color.White
            };
            turretLevelControl._x = _x + Width;
            turretLevelControl._y = _y;
            turretLevelControl.Initialize();
            base.Initialize();
        }

        public void SetTurretAnimation(Guid id)
        {
            var textureSet = TextureManager.GetTextureSet(id);
            TileSet = textureSet.LoadedContents;
            FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
            Initialize();
        }

        public void UpgradeTurretLevels(TurretInstance turret)
        {
            turretLevelControl.Text = $"{turret.GetDefinition().Upgrades.Count(x => turret.HasUpgrades.Contains(x.ID))}";
            if (turretLevelControl.Text != "0")
                turretLevelControl.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            baseControl.Update(gameTime);
            turretLevelControl.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            baseControl.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            turretLevelControl.Draw(gameTime, spriteBatch);
        }
    }
}
