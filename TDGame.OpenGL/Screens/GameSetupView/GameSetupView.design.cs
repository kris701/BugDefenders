using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.GameStyles;
using TDGame.Core.Maps;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private ButtonControl _startButton;
        private TileControl _mapPreviewTile;
        private LabelControl _mapNameLabel;
        private TextboxControl _mapDescriptionTextbox;

        public override void Initialize()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureBuilder.GetTexture(new Guid("756430ea-46e1-4c3d-95a0-a232919b7876")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(this)
            {
                Text = "Game Setup",
                Font = BasicFonts.GetFont(48),
                FontColor = Color.White,
                HorizontalAlignment = Alignment.Middle,
                Y = 25,
                Width = 100,
                Height = 35
            });

            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new MainMenu.MainMenu(Parent)))
            {
                Text = "Back",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
                X = 10,
                Y = 50,
                Width = 100,
                Height = 35
            });
            _startButton = new ButtonControl(this, clicked: StartButton_Click)
            {
                Text = "Start",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
                X = 890,
                Y = 50,
                Width = 100,
                Height = 35
            };
            AddControl(0, _startButton);

            SetupPreviewPanel();
            SetupMapsView();
            SetupGameStyleView();

#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new GameSetupView(Parent)))
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
            AddControl(0, new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                Alpha = 100,
                X = 10,
                Y = 100,
                Height = 400,
                Width = 400
            });
            _mapPreviewTile = new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Black),
                X = 20,
                Y = 110,
                Width = 380,
                Height = 380,
            };
            AddControl(1, new BorderControl(this)
            {
                Thickness = 3,
                Child = _mapPreviewTile
            });

            AddControl(0, new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                Alpha = 100,
                X = 420,
                Y = 100,
                Height = 400,
                Width = 570
            });
            _mapNameLabel = new LabelControl(this)
            {
                Text = "Select A Map",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
                X = 420,
                Y = 100,
                Height = 35,
                Width = 570
            };
            AddControl(1, _mapNameLabel);
            _mapDescriptionTextbox = new TextboxControl(this)
            {
                Font = BasicFonts.GetFont(10),
                X = 430,
                Y = 140,
                Height = 350,
                Width = 550
            };
            AddControl(1, _mapDescriptionTextbox);
        }

        private void SetupMapsView()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                Alpha = 100,
                X = 10,
                Y = 510,
                Height = 450,
                Width = 485
            });
            AddControl(1, new LabelControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
                Font = BasicFonts.GetFont(24),
                Text = "Maps",
                X = 10,
                Y = 510,
                Height = 45,
                Width = 485
            });

            var maps = MapBuilder.GetMaps();
            int offset = 0;
            foreach (var mapName in maps)
            {
                AddControl(1, new BorderControl(this)
                {
                    Thickness = 2,
                    Child = new ButtonControl(this, clicked: SelectMap_Click)
                    {
                        FillColor = BasicTextures.GetBasicRectange(Color.DarkGray),
                        FillClickedColor = BasicTextures.GetClickedTexture(),
                        Font = BasicFonts.GetFont(16),
                        Text = $"{MapBuilder.GetMap(mapName).Name}",
                        X = 20,
                        Y = 560 + offset * 40,
                        Height = 35,
                        Width = 465,
                        Tag = mapName
                    }
                });
                offset++;
            }
        }

        private void SetupGameStyleView()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                Alpha = 100,
                X = 505,
                Y = 510,
                Height = 450,
                Width = 485
            });
            AddControl(1, new LabelControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
                Font = BasicFonts.GetFont(24),
                Text = "Game Styles",
                X = 505,
                Y = 510,
                Height = 45,
                Width = 485
            });

            var gameStyles = GameStyleBuilder.GetGameStyles();
            int offset = 0;
            foreach (var gameStyle in gameStyles)
            {
                AddControl(1, new BorderControl(this)
                {
                    Thickness = 2,
                    Child = new ButtonControl(this, clicked: SelectGameStyle_Click)
                    {
                        FillColor = BasicTextures.GetBasicRectange(Color.DarkGray),
                        FillClickedColor = BasicTextures.GetClickedTexture(),
                        Font = BasicFonts.GetFont(16),
                        Text = $"{GameStyleBuilder.GetGameStyle(gameStyle).Name}",
                        X = 515,
                        Y = 560 + offset * 40,
                        Height = 35,
                        Width = 465,
                        Tag = gameStyle
                    }
                });
                offset++;
            }
        }
    }
}
