using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TDGame.Core;
using TDGame.Core.Enemies;
using TDGame.Core.Maps;
using TDGame.Core.Turret;
using TDGame.Core.Turrets;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private Rectangle _gameArea = new Rectangle(10, 10, 650, 650);

        private EntityUpdater<TurretDefinition> _turretUpdater;
        private EntityUpdater<EnemyDefinition> _enemyUpdater;
        private EntityUpdater<ProjectileDefinition> _projectile;

        private string _currentMap;
        private string _currentGameStyle;
        private Core.Game _game;
        private string _buyingTurret;

        public PathTestScreen(GameEngine parent, string map, string gamestyle) : base(parent)
        {
            _currentGameStyle = gamestyle;
            _currentMap = map;
            ScaleValue = parent.Scale;
            _game = new Core.Game(map, gamestyle);
            _turretUpdater = new EntityUpdater<TurretDefinition>(4, this, 50);
            _enemyUpdater = new EntityUpdater<EnemyDefinition>(3, this, 30);
            _projectile = new EntityUpdater<ProjectileDefinition>(5, this, 20);
            Initialize();
        }

        private void StartButton_Click(ButtonControl parent)
        {
            _game.Running = !_game.Running;
            if (_game.Running)
                _startButton.Text = "Pause";
            else
                _startButton.Text = "Start";
        }

        private void AutoRunButton_Click(ButtonControl parent)
        {
            _game.AutoSpawn = !_game.AutoSpawn;
            if (_game.AutoSpawn)
                _autoRunButton.Text = "[X] Auto-Wave";
            else
                _autoRunButton.Text = "[ ] Auto-Wave";
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (_game.GameOver)
                Parent.SwitchView(new MainMenu.MainMenu(Parent));

            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            _game.Update(gameTime.ElapsedGameTime);
            
            _moneyLabel.Text = $"Money: {_game.Money}$";
            _hpLabel.Text = $"HP: {_game.HP}";

            UpdateLasers();
            UpdateTurretPurchaseButtons();

            _turretUpdater.UpdateEntities(_game.Turrets);
            _enemyUpdater.UpdateEntities(_game.CurrentEnemies);
            _projectile.UpdateEntities(_game.Projectiles);

            if (mouseState.X >= Scale(_gameArea.X) && mouseState.X <= Scale(_gameArea.X) + Scale(_gameArea.Width) &&
                mouseState.Y >= Scale(_gameArea.Y) && mouseState.Y <= Scale(_gameArea.Y) + Scale(_gameArea.Height))
            {
                var relative = new WayPoint(InvScale(mouseState.X - Scale(_gameArea.X)), InvScale(mouseState.Y - Scale(_gameArea.Y)));
                UpdateWithinGameField(mouseState, relative, keyState);
            }
            else
            {
                _buyingPreviewTile.IsVisible = false;
            }
        }

        private void UpdateTurretPurchaseButtons()
        {
            foreach (var turret in _turretButtons)
            {
                if (turret.Tag is string turretName)
                {
                    if (_game.Money < TurretBuilder.GetTurret(turretName).Cost)
                        turret.IsEnabled = false;
                    else
                        turret.IsEnabled = true;
                }
            }
        }

        private void UpdateWithinGameField(MouseState mouseState, WayPoint relativeMousePosition, KeyboardState keyState)
        {
            if (_buyingTurret != "")
            {
                _buyingPreviewTile.IsVisible = true;
                _buyingPreviewTile._x = mouseState.X - _buyingPreviewTile.Width / 2;
                _buyingPreviewTile._y = mouseState.Y - _buyingPreviewTile.Height / 2;
                _buyingPreviewRangeTile.IsVisible = true;
                _buyingPreviewRangeTile._x = mouseState.X - Scale(_buyingPreviewRangeTile.FillColor.Width) / 2;
                _buyingPreviewRangeTile._y = mouseState.Y - Scale(_buyingPreviewRangeTile.FillColor.Height) / 2;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (_game.AddTurret(TurretBuilder.GetTurret(_buyingTurret), relativeMousePosition))
                    {
                        if (!keyState.IsKeyDown(Keys.LeftShift))
                        {
                            _buyingTurret = "";
                            _buyingPreviewTile.IsVisible = false;
                            _buyingPreviewRangeTile.IsVisible = false;
                        }
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _buyingTurret = "";
                    _buyingPreviewTile.IsVisible = false;
                    _buyingPreviewRangeTile.IsVisible = false;
                }
            }
        }

        private void UpdateLasers()
        {
            ClearLayer(6);
            foreach(var turret in _game.Turrets)
            {
                if (turret.Type == TurretType.Laser)
                {
                    if (turret.Targeting != null)
                    {
                        AddControl(6, new LineControl(this)
                        {
                            Thickness = 3,
                            Stroke = BasicTextures.GetBasicRectange(Color.Red),
                            X = turret.X,
                            Y = turret.Y,
                            X2 = turret.Targeting.X,
                            Y2 = turret.Targeting.Y,
                        });
                    }
                }
            }
        }

        private void BuyTurret_Click(ButtonControl parent)
        {
            if (parent.Tag is string turretName)
            {
                _buyingTurret = turretName;
                var turret = TurretBuilder.GetTurret(turretName);
                _buyingPreviewTile.FillColor = TextureBuilder.GetTexture(turret.ID);
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, turret.Range);
            }
        }
    }
}
