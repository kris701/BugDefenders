using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Maps;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EnemyControl : AnimatedTileControl
    {
        public FloatPoint VisualOffset { get; }
        private TileControl _healthBar;
        private readonly float _initialHP;
        private readonly EnemyInstance _enemy;
        private readonly int _legalOffset = 5;
        public EnemyControl(UIEngine parent, EnemyInstance enemy) : base(parent)
        {
            _initialHP = enemy.Health;
            _enemy = enemy;
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
            _healthBar._width = _width * (_enemy.Health / _initialHP);
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
