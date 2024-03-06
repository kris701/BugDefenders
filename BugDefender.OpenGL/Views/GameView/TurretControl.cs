using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using BugDefender.OpenGL.Screens.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static BugDefender.OpenGL.Engine.Controls.AnimatedButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class TurretControl : CollectionControl
    {
        public TurretInstance Instance { get; }
        public Guid CurrentSoundEffect { get; set; }

        private readonly GameWindow _parent;
        private readonly AnimatedButtonControl _turretControl;
        private readonly LabelControl _turretLevelControl;
        private Guid _currentAnimation;
        public TurretControl(GameWindow parent, TurretInstance instance, ClickedHandler clicked)
        {
            _parent = parent;
            Instance = instance;
            Width = instance.Size;
            Height = instance.Size;

            Children.Add(new TileControl()
            {
                X = (Width - 35) / 2,
                Y = (Height - 35) / 2,
                FillColor = parent.UIResources.GetTexture(new Guid("ba2a23be-8bf7-4307-9009-8ed330ac5b7d")),
                Width = 35,
                Height = 35,
            });
            _currentAnimation = parent.UIResources.GetAnimation<TurretEntityDefinition>(instance.DefinitionID).OnIdle;
            var textureSet = parent.UIResources.GetTextureSet(_currentAnimation);
            _turretControl = new AnimatedButtonControl(parent, clicked)
            {
                TileSet = textureSet.LoadedContents,
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Transparent),
                FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent),
                Rotation = instance.Angle + (float)Math.PI / 2,
                Tag = instance
            };
            Children.Add(_turretControl);
            _turretLevelControl = new LabelControl()
            {
                X = Width,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                IsVisible = Instance.HasUpgrades.Count > 0,
                Text = $"{Instance.HasUpgrades.Count}"
            };
            Children.Add(_turretLevelControl);
        }

        public void SetTurretAnimation(Guid id)
        {
            if (id == _currentAnimation)
                return;
            _currentAnimation = id;
            var textureSet = _parent.UIResources.GetTextureSet(id);
            _turretControl.TileSet = textureSet.LoadedContents;
            _turretControl.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
        }

        public void UpgradeTurretLevels(TurretInstance turret)
        {
            if (turret.HasUpgrades.Count > 0)
            {
                _turretLevelControl.Text = $"{turret.HasUpgrades.Count}";
                _turretLevelControl.IsVisible = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _turretControl.Rotation = Rotation;
            base.Update(gameTime);
        }
    }
}
