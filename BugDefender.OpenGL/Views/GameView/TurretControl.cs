﻿using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using System;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class TurretControl : CollectionControl
    {
        public TurretInstance Instance { get; }
        public Guid CurrentSoundEffect { get; set; }

        private readonly BugDefenderGameWindow _parent;
        private readonly BugDefenderAnmiatedButtonControl _turretControl;
        private readonly LabelControl _turretLevelControl;
        private Guid _currentAnimation;
        public TurretControl(BugDefenderGameWindow parent, TurretInstance instance, ClickedHandler clicked)
        {
            _parent = parent;
            Instance = instance;
            Width = instance.Size;
            Height = instance.Size;

            Children.Add(new TileControl()
            {
                X = (Width - 35) / 2,
                Y = (Height - 35) / 2,
                FillColor = parent.Textures.GetTexture(new Guid("ba2a23be-8bf7-4307-9009-8ed330ac5b7d")),
                Width = 35,
                Height = 35,
            });
            _currentAnimation = parent.ResourcePackController.GetAnimation<TurretEntityDefinition>(instance.DefinitionID).OnIdle;
            var textureSet = parent.Textures.GetTextureSet(_currentAnimation);
            _turretControl = new BugDefenderAnmiatedButtonControl(parent, clicked)
            {
                TileSet = textureSet.GetLoadedContent(),
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Transparent),
                FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent),
                Rotation = instance.Angle + (float)Math.PI / 2,
                Width = Width,
                Height = Height,
                Tag = instance
            };
            Children.Add(_turretControl);
            _turretLevelControl = new LabelControl()
            {
                X = Width,
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
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
            var textureSet = _parent.Textures.GetTextureSet(id);
            _turretControl.TileSet = textureSet.GetLoadedContent();
            _turretControl.Frame = 0;
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
