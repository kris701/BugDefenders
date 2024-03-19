using BugDefender.Core.Campaign.Models;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;

namespace BugDefender.OpenGL.Screens.CampaignGameSetupView
{
    public partial class CampaignGameSetupView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("1ccc48ee-6738-45cd-ae14-50d3d0896dc0");

        private CampaignDefinition? _selectedCampaign;
        private readonly KeyWatcher _escapeKeyWatcher;

        public CampaignGameSetupView(BugDefenderGameWindow parent) : base(parent, _id)
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
            if (_selectedCampaign != null)
            {
                Parent.GameManager.NewGame(new CampaignSavedGame(_gameSaveName.Text, DateTime.Now, null, _selectedCampaign.ID, Guid.Empty, false, new Core.Users.Models.StatsDefinition(), false));
            }
        }

        private void NameKeyDown(TextInputControl sender)
        {
            if (Parent.UserManager.SaveExists(sender.Text))
                _saveOverwriteWarningLabel.IsVisible = true;
            else
                _saveOverwriteWarningLabel.IsVisible = false;
        }

        private void SelectCampaign_Click(ButtonControl sender)
        {
            if (sender is BugDefenderButtonControl button)
            {
                if (sender.Tag is CampaignDefinition campaign)
                {
                    if (_selectedCampaignButton != null)
                        _selectedCampaignButton.FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));

                    _selectedCampaignButton = button;
                    _selectedCampaignButton.FillColor = Parent.TextureController.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));

                    _selectedCampaign = campaign;
                    _campaignPreviewTile.FillColor = Parent.TextureController.GetTexture(campaign.ID);
                    _campaignNameLabel.Text = campaign.Name;
                    var sb = new StringBuilder();
                    sb.AppendLine(campaign.Description);
                    sb.AppendLine($"Completion reward: {_selectedCampaign.Reward} credits!");
                    _campaignDescriptionTextbox.Text = sb.ToString();
                    _startButton.IsEnabled = true;
                }
            }
        }
    }
}
