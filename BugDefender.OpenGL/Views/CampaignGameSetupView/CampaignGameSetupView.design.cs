using BugDefender.Core.Resources;
using BugDefender.OpenGL.Controls;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BugDefender.OpenGL.Helpers;

namespace BugDefender.OpenGL.Screens.CampaignGameSetupView
{
    public partial class CampaignGameSetupView : BaseBugDefenderView
    {
        private TileControl _campaignPreviewTile;
        private LabelControl _campaignNameLabel;
        private TextboxControl _campaignDescriptionTextbox;
        private BugDefenderButtonControl _startButton;
        private TextInputControl _gameSaveName;
        private LabelControl _saveOverwriteWarningLabel;

        private BugDefenderButtonControl? _selectedCampaignButton;

        private PageHandler<BugDefenderButtonControl> _campaignPageHandler;

        [MemberNotNull(nameof(_campaignPreviewTile), nameof(_campaignNameLabel), nameof(_campaignDescriptionTextbox),
            nameof(_startButton), nameof(_campaignPageHandler),
            nameof(_gameSaveName),
            nameof(_saveOverwriteWarningLabel))]
        public override void Initialize()
        {
            BasicMenuHelper.GenerateBaseMenu(
                this,
                Parent.Textures.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Campaign Game Setup",
                "Select a campaign to start playing!",
                Parent.Fonts);

            SetupPreviewPanel(200, 225, 1060, 750);
            SetupCampaignsView(1060 + 200 + 10, 225, 445, 750);

            _startButton = BasicMenuHelper.GetAcceptButton(Parent, "Start", StartButton_Click);
            _startButton.IsEnabled = false;
            AddControl(0, _startButton);
            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Back", (e) => { SwitchView(new MainMenu.MainMenuView(Parent)); }));

#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new CampaignGameSetupView(Parent)))
            {
                X = 0,
                Y = 0,
                Width = 50,
                Height = 25,
                Text = "Reload",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
#endif

            base.Initialize();
        }

        [MemberNotNull(nameof(_campaignPreviewTile), nameof(_campaignNameLabel), nameof(_campaignDescriptionTextbox), nameof(_gameSaveName), nameof(_saveOverwriteWarningLabel))]
        private void SetupPreviewPanel(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.Textures.GetTexture(new Guid("02f8c9e2-e4c0-4310-934a-62c84cbb7384")),
            });
            _campaignPreviewTile = new TileControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Black),
                X = x + 50,
                Y = y + 170,
                Width = 960,
                Height = 540,
            };
            AddControl(1, new BorderControl(_campaignPreviewTile)
            {
                Thickness = 3,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue)
            });

            _campaignNameLabel = new LabelControl()
            {
                Text = "Select A Campaign",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                X = x + 50,
                Y = y + 10,
                Height = 50,
                Width = (width - 100) / 2,
                FontColor = Color.White,
            };
            AddControl(1, _campaignNameLabel);
            _campaignDescriptionTextbox = new TextboxControl()
            {
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                X = x + 50,
                Y = y + 60,
                Height = 100,
                Width = (width - 100) / 2,
                FontColor = Color.White
            };
            AddControl(1, _campaignDescriptionTextbox);

            AddControl(1, new LabelControl()
            {
                X = x + 50 + (width - 100) / 2,
                Y = y + 10,
                Width = (width - 100) / 2,
                Height = 50,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                Text = "New Save Name",
                FontColor = Color.White
            });
            _gameSaveName = new TextInputControl(Parent)
            {
                X = x + 50 + (width - 100) / 2,
                Y = y + 60,
                Height = 50,
                Width = (width - 100) / 2,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                Text = "New Game",
                Limit = 25,
                FontColor = Color.White,
                FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = Parent.Textures.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
            };
            _gameSaveName.OnKeyDown += NameKeyDown;
            AddControl(1, _gameSaveName);
            _saveOverwriteWarningLabel = new LabelControl()
            {
                X = x + 50 + (width - 100) / 2,
                Y = y + 110,
                Height = 50,
                Width = (width - 100) / 2,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                Text = "Overwrites existing save!",
                FontColor = Color.Red,
                IsVisible = Parent.UserManager.SaveExists(_gameSaveName.Text)
            };
            AddControl(1, _saveOverwriteWarningLabel);
        }

        [MemberNotNull(nameof(_campaignPageHandler))]
        private void SetupCampaignsView(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.Textures.GetTexture(new Guid("e5cb13c4-39e1-4906-b1d1-52e353fb0546")),
            });
            AddControl(1, new LabelControl()
            {
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                Text = "Campaigns",
                X = x,
                Y = y + 10,
                Height = 50,
                Width = width,
                FontColor = Color.White
            });

            var controlList = new List<BugDefenderButtonControl>();
            var ids = ResourceManager.Campaigns.GetResources();
            foreach (var id in ids)
            {
                var map = ResourceManager.Campaigns.GetResource(id);
                controlList.Add(new BugDefenderButtonControl(Parent, SelectCampaign_Click)
                {
                    FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                    Text = $"{map.Name}",
                    FontColor = Color.White,
                    Height = 50,
                    Width = width - 20,
                    Tag = map
                });
            }
            _campaignPageHandler = new PageHandler<BugDefenderButtonControl>(this, controlList)
            {
                LeftButtonX = 10,
                LeftButtonY = -50,
                RightButtonX = width - 80,
                RightButtonY = -50,
                ItemsPrPage = 12,
                X = x + 10,
                Y = y + 70,
                Width = width,
                Height = height
            };
            AddControl(1, _campaignPageHandler);
        }
    }
}
