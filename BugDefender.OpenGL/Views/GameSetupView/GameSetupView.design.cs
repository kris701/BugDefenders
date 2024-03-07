using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseBugDefenderView
    {
        private TileControl _mapPreviewTile;
        private LabelControl _mapNameLabel;
        private TextboxControl _mapDescriptionTextbox;
        private ButtonControl _startButton;

        private PageHandler<ButtonControl> _mapPageHandler;
        private PageHandler<ButtonControl> _gamestylePageHandler;

        public override void Initialize()
        {
            BasicMenuPage.GenerateBaseMenu(
                this,
                Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Game Setup",
                "Select a map and a gamestyle to start.");

            SetupPreviewPanel(50, 225, 900, 750);
            SetupMapsView(965, 225, 445, 750);
            SetupGameStyleView(1425, 225, 445, 750);

            _startButton = new ButtonControl(Parent, StartButton_Click)
            {
                X = 50,
                Y = 980,
                Width = 200,
                Height = 50,
                Text = "Start",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = Parent.TextureController.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                IsEnabled = false
            };
            AddControl(0, _startButton);
            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 980,
                X = 1670,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new GameSetupView(Parent)))
            {
                X = 0,
                Y = 0,
                Width = 50,
                Height = 25,
                Text = "Reload",
                Font = BasicFonts.GetFont(10),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
#endif

            base.Initialize();
        }

        private void SetupPreviewPanel(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.TextureController.GetTexture(new Guid("02f8c9e2-e4c0-4310-934a-62c84cbb7384")),
            });
            _mapPreviewTile = new TileControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Black),
                X = x + 50,
                Y = y + 150,
                Width = height - 300,
                Height = height - 300,
            };
            AddControl(1, new BorderControl(_mapPreviewTile)
            {
                Thickness = 3,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue)
            });

            _mapNameLabel = new LabelControl()
            {
                Text = "Select A Map",
                Font = BasicFonts.GetFont(16),
                X = x + 50,
                Y = y + 50,
                Height = 50,
                Width = height - 300,
                FontColor = Color.White,
            };
            AddControl(1, _mapNameLabel);
            _mapDescriptionTextbox = new TextboxControl()
            {
                Font = BasicFonts.GetFont(10),
                X = x + (height - 300) + 75,
                Y = y + 50,
                Height = height - 100,
                Width = width - 550,
                FontColor = Color.White
            };
            AddControl(1, _mapDescriptionTextbox);
        }

        private void SetupMapsView(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.TextureController.GetTexture(new Guid("e5cb13c4-39e1-4906-b1d1-52e353fb0546")),
            });
            AddControl(1, new LabelControl()
            {
                Font = BasicFonts.GetFont(24),
                Text = "Maps",
                X = x,
                Y = y + 10,
                Height = 50,
                Width = width,
                FontColor = Color.White
            });
            _mapPageHandler = new PageHandler<ButtonControl>()
            {
                LeftButtonX = x + 30,
                LeftButtonY = y + 10,
                RightButtonX = x + width - 80,
                RightButtonY = y + 10,
                ItemsPrPage = 13,
                X = x + 10,
                Y = y + 70,
            };

            var controlList = new List<ButtonControl>();
            var ids = ResourceManager.Maps.GetResources();
            foreach (var id in ids)
            {
                var map = ResourceManager.Maps.GetResource(id);
                controlList.Add(new ButtonControl(Parent, clicked: SelectMap_Click)
                {
                    FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.TextureController.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = BasicFonts.GetFont(12),
                    Text = $"{map.Name}",
                    FontColor = Color.White,
                    Height = 50,
                    Width = width - 20,
                    Tag = map
                });
            }

            _mapPageHandler.Initialize(controlList, this);
        }

        private void SetupGameStyleView(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.TextureController.GetTexture(new Guid("e5cb13c4-39e1-4906-b1d1-52e353fb0546")),
            });
            AddControl(1, new LabelControl()
            {
                Font = BasicFonts.GetFont(24),
                Text = "Game Styles",
                X = x,
                Y = y + 10,
                Height = 50,
                Width = width,
                FontColor = Color.White
            });
            _gamestylePageHandler = new PageHandler<ButtonControl>()
            {
                LeftButtonX = x + 30,
                LeftButtonY = y + 10,
                RightButtonX = x + width - 80,
                RightButtonY = y + 10,
                ItemsPrPage = 13,
                X = x + 10,
                Y = y + 70,
            };

            var controlList = new List<ButtonControl>();
            var ids = ResourceManager.GameStyles.GetResources();
            foreach (var id in ids)
            {
                var gameStyle = ResourceManager.GameStyles.GetResource(id);
                controlList.Add(new ButtonControl(Parent, clicked: SelectGameStyle_Click)
                {
                    FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.TextureController.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = BasicFonts.GetFont(12),
                    Text = $"{gameStyle.Name}",
                    FontColor = Color.White,
                    Height = 50,
                    Width = width - 20,
                    Tag = gameStyle
                });
            }

            _gamestylePageHandler.Initialize(controlList, this);
        }
    }
}
