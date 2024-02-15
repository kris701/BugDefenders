using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;
using TDGame.Core.Models.Entities.Enemies;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EnemyControl : TileControl
    {
        private TileControl _healthBar;
        private float _initialHP;
        private EnemyInstance _enemy;
        public EnemyControl(IScreen parent, EnemyInstance enemy) : base(parent)
        {
            _initialHP = enemy.Health;
            _enemy = enemy;
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
