using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Turrets;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class TurretControl : ButtonControl
    {
        private TileControl baseControl;
        private LabelControl turretLevelControl;
        private LabelControl projectileLevelControl;
        public TurretControl(IScreen parent, ClickedHandler clicked = null, ClickedHandler clickedModifierA = null, ClickedHandler clickedModifierB = null) : base(parent, clicked, clickedModifierA, clickedModifierB)
        {
        }

        public override void Initialize()
        {
            var baseTexture = TextureBuilder.GetTexture(new Guid("ba2a23be-8bf7-4307-9009-8ed330ac5b7d"));
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
                IsVisible = false
            };
            turretLevelControl._x = _x + Width;
            turretLevelControl._y = _y;
            turretLevelControl.Initialize();

            projectileLevelControl = new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                IsVisible = false
            };
            projectileLevelControl._x = _x + Width;
            projectileLevelControl._y = _y + Height;
            projectileLevelControl.Initialize();

            base.Initialize();
        }

        public void UpgradeTurretLevels(TurretDefinition turret)
        {
            turretLevelControl.Text = $"{turret.TurretLevels.Count(x => x.HasUpgrade)}";
            if (turretLevelControl.Text != "0")
                turretLevelControl.IsVisible = true;
            projectileLevelControl.Text = $"{turret.ProjectileLevels.Count(x => x.HasUpgrade)}";
            if (projectileLevelControl.Text != "0")
                projectileLevelControl.IsVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            baseControl.Update(gameTime);
            turretLevelControl.Update(gameTime);
            projectileLevelControl.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            baseControl.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            turretLevelControl.Draw(gameTime, spriteBatch);
            projectileLevelControl.Draw(gameTime, spriteBatch);
        }
    }
}
