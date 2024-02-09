using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TDGame.Core;
using TDGame.Core.Enemies;
using TDGame.Core.Maps;
using TDGame.Core.Turret;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private TDGame.Core.Game _game;

        private CanvasControl _canvas;

        public PathTestScreen(IEngine parent) : base(parent)
        {
            _game = new TDGame.Core.Game("map1", "easy");
            _game.AddTurret(TurretBuilder.GetTurret("turret1"), new WayPoint(175, 300));
            _game.AddTurret(TurretBuilder.GetTurret("turret2"), new WayPoint(225, 300));
            OnUpdate += Game_OnUpdate;
            Initialize();
        }

        private void Game_OnUpdate(GameTime gameTime)
        {
            _game.Update(gameTime.ElapsedGameTime);
            _canvas.Children.Clear();
            foreach (var blockingTile in _game.Map.BlockingTiles)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.Black,
                    Width = blockingTile.Width,
                    Height = blockingTile.Height,
                    X = blockingTile.X,
                    Y = blockingTile.Y,
                };
                _canvas.Children.Add(newPanel);
            }
            foreach (var enemy in _game.CurrentEnemies)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.White,
                    Width = 10,
                    Height = 10,
                    X = enemy.X,
                    Y = enemy.Y,
                };
                _canvas.Children.Add(newPanel);
            }
            foreach (var turret in _game.Turrets)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.Gray,
                    Width = turret.Size,
                    Height = turret.Size,
                    X = turret.X,
                    Y = turret.Y,
                };
                _canvas.Children.Add(newPanel);
            }
            foreach (var rocket in _game.Rockets)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.Gray,
                    Width = 10,
                    Height = 10,
                    X = rocket.X,
                    Y = rocket.Y,
                };
                _canvas.Children.Add(newPanel);
            }
        }
    }
}
