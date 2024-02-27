using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Views.GameView
{
    public class TurretControl : AnimatedButtonControl
    {
        public TurretInstance Instance { get; }
        public Guid CurrentSoundEffect { get; set; }

        private readonly TileControl baseControl;
        private readonly LabelControl turretLevelControl;
        private Guid _currentAnimation;
        public TurretControl(UIEngine parent, TurretInstance instance, ClickedHandler clicked) : base(parent, clicked)
        {
            Instance = instance;
            baseControl = new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("ba2a23be-8bf7-4307-9009-8ed330ac5b7d")),
                Width = 35,
                Height = 35,
            };
            turretLevelControl = new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                IsVisible = Instance.HasUpgrades.Count > 0,
                Text = $"{Instance.HasUpgrades.Count}"
            };
        }

        public override void Initialize()
        {
            baseControl._x = _x + (Width - baseControl.Width) / 2;
            baseControl._y = _y + (Height - baseControl.Height) / 2;
            baseControl.Initialize();

            turretLevelControl._x = _x + Width;
            turretLevelControl._y = _y;
            turretLevelControl.Initialize();
            base.Initialize();
        }

        public void SetTurretAnimation(Guid id)
        {
            if (id == _currentAnimation)
                return;
            _currentAnimation = id;
            var textureSet = Parent.UIResources.GetTextureSet(id);
            TileSet = textureSet.LoadedContents;
            FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
        }

        public void UpgradeTurretLevels(TurretInstance turret)
        {
            if (turret.HasUpgrades.Count > 0)
            {
                turretLevelControl.Text = $"{turret.HasUpgrades.Count}";
                turretLevelControl.IsVisible = true;
            }
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
