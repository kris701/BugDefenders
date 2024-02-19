using Microsoft.Xna.Framework;
using System;
using System.Runtime;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private TileControl _mapPreviewTile;
        private LabelControl _mapNameLabel;
        private TextboxControl _mapDescriptionTextbox;
        private ButtonControl _startButton;

        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("756430ea-46e1-4c3d-95a0-a232919b7876")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Game Setup",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Select a map and a gamemode to start.  ",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            SetupPreviewPanel();
            SetupMapsView();
            SetupGameStyleView();

            _startButton = new ButtonControl(Parent, StartButton_Click)
            {
                X = 50,
                Y = 900,
                Width = 200,
                Height = 50,
                Text = "Start",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = TextureManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = TextureManager.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                IsEnabled = false
            };
            AddControl(0, _startButton);
            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = TextureManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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

        private void SetupPreviewPanel()
        {
            _mapPreviewTile = new TileControl(Parent)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Black),
                X = 60,
                Y = 230,
                Width = 300,
                Height = 300,
            };
            AddControl(1, new BorderControl(Parent)
            {
                Thickness = 3,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue),
                Child = _mapPreviewTile
            });

            _mapNameLabel = new LabelControl(Parent)
            {
                Text = "Select A Map",
                Font = BasicFonts.GetFont(16),
                X = 450,
                Y = 220,
                Height = 50,
                Width = 400,
                FontColor = Color.White,
                FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
            };
            AddControl(1, _mapNameLabel);
            _mapDescriptionTextbox = new TextboxControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                X = 380,
                Y = 270,
                Height = 250,
                Width = 550,
                FontColor = Color.White
            };
            AddControl(1, _mapDescriptionTextbox);
        }

        private void SetupMapsView()
        {
            AddControl(1, new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(24),
                Text = "Maps",
                X = 70,
                Y = 550,
                Height = 50,
                Width = 400,
                FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FontColor = Color.White
            });

            var maps = ResourceManager.Maps.GetResources();
            int offset = 0;
            foreach (var mapName in maps)
            {
                AddControl(1, new ButtonControl(Parent, clicked: SelectMap_Click)
                {
                    FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = TextureManager.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = BasicFonts.GetFont(16),
                    Text = $"{ResourceManager.Maps.GetResource(mapName).Name}",
                    FontColor = Color.White,
                    X = 70,
                    Y = 610 + offset * 50,
                    Height = 50,
                    Width = 400,
                    Tag = mapName
                });
                offset++;
            }
        }

        private void SetupGameStyleView()
        {
            AddControl(1, new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(24),
                Text = "Game Styles",
                X = 520,
                Y = 550,
                Height = 50,
                Width = 400,
                FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FontColor = Color.White
            });

            var gameStyles = ResourceManager.GameStyles.GetResources();
            int offset = 0;
            foreach (var gameStyle in gameStyles)
            {
                AddControl(1, new ButtonControl(Parent, clicked: SelectGameStyle_Click)
                {
                    FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = TextureManager.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    FontColor = Color.White,
                    Font = BasicFonts.GetFont(16),
                    Text = $"{ResourceManager.GameStyles.GetResource(gameStyle).Name}",
                    X = 520,
                    Y = 610 + offset * 50,
                    Height = 50,
                    Width = 400,
                    Tag = gameStyle
                });
                offset++;
            }
        }
    }
}
