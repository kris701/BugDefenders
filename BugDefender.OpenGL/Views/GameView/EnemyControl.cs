using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EnemyControl : AnimatedTileControl
    {
        public FloatPoint VisualOffset { get; }
        public EnemyInstance Enemy { get; }
        private TileControl _healthBar;
        private readonly float _initialHP;
        private readonly int _legalOffset = 5;
        public EnemyControl(UIEngine parent, EnemyInstance enemy) : base(parent)
        {
            _initialHP = enemy.Health;
            Enemy = enemy;
            var rnd = new Random();
            VisualOffset = new FloatPoint(
                rnd.Next(-_legalOffset, _legalOffset),
                rnd.Next(-_legalOffset, _legalOffset)
                );
        }

        public override void Initialize()
        {
            base.Initialize();
            _healthBar = new TileControl(Parent)
            {
                Width = Width,
                Height = 5,
                FillColor = BasicTextures.GetBasicRectange(Color.Green)
            };
            _healthBar._x = _x;
            _healthBar._y = _y - 5;
        }

        public void SetEnemyAnimation(Guid id)
        {
            var textureSet = Parent.UIResources.GetTextureSet(id);
            TileSet = textureSet.LoadedContents;
            FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
            Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _healthBar._width = _width * (Enemy.Health / _initialHP);
            _healthBar._x = _x;
            _healthBar._y = _y - 5;

            _healthBar.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            _healthBar.Draw(gameTime, spriteBatch);
        }
    }
}
