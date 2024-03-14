using BugDefender.Core.Campain.Models;
using BugDefender.Core.Game;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.GameView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Xml.Linq;

namespace BugDefender.OpenGL.Screens.CampainGameSetupView
{
    public partial class CampainGameSetupView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("1ccc48ee-6738-45cd-ae14-50d3d0896dc0");

        private CampainDefinition? _selectedCampain;
        private readonly KeyWatcher _escapeKeyWatcher;

        public CampainGameSetupView(BugDefenderGameWindow parent) : base(parent, _id)
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
            if (_selectedCampain != null)
            {
                var gameHandler = new GameHandler(Parent);
                gameHandler.LoadGame(
                    this,
                    new CampainSavedGame(_gameSaveName.Text, DateTime.Now, null, _selectedCampain.ID, Guid.Empty, new Core.Users.Models.StatsDefinition(), false));
            }
        }

        private void NameKeyDown(TextInputControl sender)
        {
            if (Parent.UserManager.SaveExists(sender.Text))
                _saveOverwriteWarningLabel.IsVisible = true;
            else
                _saveOverwriteWarningLabel.IsVisible = false;
        }

        private void SelectCampain_Click(ButtonControl sender)
        {
            if (sender is BugDefenderButtonControl button)
            {
                if (sender.Tag is CampainDefinition campain)
                {
                    if (_selectedCampainButton != null)
                        _selectedCampainButton.FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                    _selectedCampainButton = button;
                    _selectedCampainButton.FillColor = Parent.TextureController.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                    _selectedCampain = campain;
                    _campainPreviewTile.FillColor = Parent.TextureController.GetTexture(campain.ID);
                    _campainNameLabel.Text = campain.Name;
                    var sb = new StringBuilder();
                    sb.AppendLine(campain.Description);
                    sb.AppendLine($"Completion reward: {_selectedCampain.Reward} credits!");
                    _campainDescriptionTextbox.Text = sb.ToString();
                    _startButton.IsEnabled = true;
                }
            }
        }
    }
}
