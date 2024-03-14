﻿using BugDefender.Core.Game;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Xml.Linq;

namespace BugDefender.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("1ccc48ee-6738-45cd-ae14-50d3d0896dc0");

        private GameStyleDefinition? _selectedGameStyle;
        private MapDefinition? _selectedMap;
        private readonly KeyWatcher _escapeKeyWatcher;

        public GameSetupView(BugDefenderGameWindow parent) : base(parent, _id)
        {
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }

        private void StartButton_Click(ButtonControl sender)
        {
            if (_selectedMap != null && _selectedGameStyle != null)
                SwitchView(new GameScreen.GameScreen(Parent, new SurvivalSavedGame(_gameSaveName.Text, DateTime.Now, new GameContext(_selectedMap.ID, _selectedGameStyle.ID)), OnGameOver));
        }

        private void NameKeyDown(TextInputControl sender)
        {
            if (Parent.UserManager.SaveExists(sender.Text))
                _saveOverwriteWarningLabel.IsVisible = true;
            else
                _saveOverwriteWarningLabel.IsVisible = false;
        }

        private void OnGameOver(GameEngine game, ISavedGame gameSave)
        {
            var credits = 0;
            var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
                if (result == GameResult.Success)
                {
                    Parent.UserManager.CurrentUser.Stats.Combine(game.Context.Stats);
                    credits += (game.Context.Score / 100);
                    Parent.UserManager.CurrentUser.Credits += credits;
                    Parent.UserManager.CheckAndApplyAchivements();
                    Parent.UserManager.SaveUser();
                }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            SwitchView(new GameOverScreen.GameOverView(Parent, screen, game.Context, credits, game.Result, game.Context.Map.GetDifficultyRating() * game.Context.GameStyle.GetDifficultyRating(), "Game Over!"));
        }

        private void SelectMap_Click(ButtonControl sender)
        {
            if (sender is BugDefenderButtonControl button)
            {
                if (sender.Tag is MapDefinition map)
                {
                    if (_selectedMapButton != null)
                        _selectedMapButton.FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                    _selectedMapButton = button;
                    _selectedMapButton.FillColor = Parent.TextureController.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                    _selectedMap = map;
                    _mapPreviewTile.FillColor = Parent.TextureController.GetTexture(map.ID);
                    _mapNameLabel.Text = map.Name;
                    var sb = new StringBuilder();
                    sb.AppendLine(map.Description);
                    if (map.Tags.Count > 0)
                    {
                        sb.AppendLine("Tags:");
                        foreach (var tag in map.Tags)
                            sb.Append($"{tag}, ");
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                    sb.AppendLine($"Difficulty Rating: {Math.Round(map.GetDifficultyRating(), 2)}");
                    _mapDescriptionTextbox.Text = sb.ToString();

                    if (_selectedGameStyle != null && _selectedMap != null)
                    {
                        _totalDifficultyLabel.Text = $"Total Difficulty: {Math.Round(_selectedGameStyle.GetDifficultyRating() * _selectedMap.GetDifficultyRating(), 2)}";
                        _startButton.IsEnabled = true;
                    }
                }
            }
        }

        private void SelectGameStyle_Click(ButtonControl sender)
        {
            if (sender is BugDefenderButtonControl button)
            {
                if (sender.Tag is GameStyleDefinition gameStyle)
                {
                    if (_selectedGameStyleButton != null)
                        _selectedGameStyleButton.FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                    _selectedGameStyleButton = button;
                    _selectedGameStyleButton.FillColor = Parent.TextureController.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                    var sb = new StringBuilder();
                    sb.AppendLine(gameStyle.Description);
                    sb.AppendLine();
                    sb.AppendLine($"Difficulty Rating: {Math.Round(gameStyle.GetDifficultyRating(), 2)}");
                    _gameStyleDescriptionTextbox.Text = sb.ToString();

                    _selectedGameStyle = gameStyle;

                    if (_selectedGameStyle != null && _selectedMap != null)
                    {
                        _totalDifficultyLabel.Text = $"Total Difficulty: {Math.Round(_selectedGameStyle.GetDifficultyRating() * _selectedMap.GetDifficultyRating(), 2)}";
                        _startButton.IsEnabled = true;
                    }
                }
            }
        }
    }
}
