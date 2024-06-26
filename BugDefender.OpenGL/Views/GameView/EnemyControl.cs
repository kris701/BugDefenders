﻿using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using System;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EnemyControl : CollectionControl
    {
        public FloatPoint VisualOffset { get; }
        public EnemyInstance Enemy { get; }

        private readonly TileControl _healthBar;
        private readonly AnimatedTileControl _enemyTile;
        private readonly float _initialHP;
        private readonly int _legalOffset = 5;
        public EnemyControl(BugDefenderGameWindow parent, EnemyInstance enemy)
        {
            Width = enemy.Size;
            Height = enemy.Size;

            _initialHP = enemy.Health;
            Enemy = enemy;
            var rnd = new Random();
            VisualOffset = new FloatPoint(
                rnd.Next(-_legalOffset, _legalOffset),
                rnd.Next(-_legalOffset, _legalOffset)
                );

            var animation = parent.ResourcePackController.GetAnimation<EnemyEntityDefinition>(enemy.DefinitionID).OnCreate;
            var textureSet = parent.Textures.GetTextureSet(animation);
            _enemyTile = new AnimatedTileControl()
            {
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                TileSet = textureSet.GetLoadedContent(),
                AutoPlay = true,
                Width = enemy.Size,
                Height = enemy.Size,
                Rotation = enemy.Angle + (float)Math.PI / 2,
                Tag = enemy
            };
            Children.Add(_enemyTile);
            _healthBar = new TileControl()
            {
                Width = Width,
                Height = 5,
                Y = -5,
                FillColor = BasicTextures.GetBasicRectange(Color.Green)
            };
            Children.Add(_healthBar);
        }

        public override void Update(GameTime gameTime)
        {
            _enemyTile.X = X;
            _enemyTile.Y = Y;
            _enemyTile.Rotation = Rotation;
            _healthBar.X = X;
            _healthBar.Y = Y - 5;
            _healthBar.Width = Width * (Enemy.Health / _initialHP);

            base.Update(gameTime);
        }
    }
}
