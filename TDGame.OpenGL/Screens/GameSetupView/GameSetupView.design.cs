using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Screens.MainMenu;
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

namespace TDGame.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private TileControl _mapPreviewTile;

        public override void Initialize()
        {
#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => Parent.SwitchView(new GameSetupView(Parent)))
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

            AddControl(0, new ButtonControl(this, clicked: (x) => Parent.SwitchView(new MainMenu(Parent)))
            {
                Text = "Back",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                X = 10,
                Y = 50,
                Width = 100,
                Height = 35
            });
            AddControl(0, new ButtonControl(this, clicked: StartButton_Click)
            {
                Text = "Start",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                X = 890,
                Y = 50,
                Width = 100,
                Height = 35
            });

            AddControl(0, new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Blue),
                Alpha = 100,
                X = 10,
                Y = 100,
                Height = 400,
                Width = 400
            });
            _mapPreviewTile = new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Aqua),
                X = 20,
                Y = 110,
                Width = 380,
                Height = 380,
                ForceFit = true
            };
            AddControl(1, new BorderControl(this)
            {
                Thickness = 3,
                Child = _mapPreviewTile
            });

            AddControl(0, new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Blue),
                Alpha = 100,
                X = 420,
                Y = 100,
                Height = 400,
                Width = 570
            });

            SetupMapsView();
            SetupGameStyleView();

            base.Initialize();
        }

        private void SetupMapsView()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Green),
                Alpha = 100,
                X = 10,
                Y = 510,
                Height = 450,
                Width = 485
            });
            AddControl(1, new LabelControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
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
                    Thickness = 4,
                    Child = new ButtonControl(this, clicked: SelectMap_Click)
                    {
                        FillColor = BasicTextures.GetBasicRectange(Color.DarkRed),
                        FillClickedColor = BasicTextures.GetBasicRectange(Color.Magenta),
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
                FillColor = BasicTextures.GetBasicRectange(Color.Green),
                Alpha = 100,
                X = 505,
                Y = 510,
                Height = 450,
                Width = 485
            });
            AddControl(1, new LabelControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
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
                    Thickness = 4,
                    Child = new ButtonControl(this, clicked: SelectGameStyle_Click)
                    {
                        FillColor = BasicTextures.GetBasicRectange(Color.DarkRed),
                        FillClickedColor = BasicTextures.GetBasicRectange(Color.Magenta),
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
