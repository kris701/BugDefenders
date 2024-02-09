using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Screens.MainMenu;
using Project1.Screens.PathTest;
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

namespace Project1.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private StackPanelControl _mapSelectionStackPanel;
        private StackPanelControl _gameStyleSelectionStackPanel;
        private CanvasControl _mapPreviewPanel;

        public override void Initialize()
        {
            _mapSelectionStackPanel = new StackPanelControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                Margin = 5,
                Row = 1,
                Column = 1,
                Children = new List<IControl>()
                {
                    new LabelControl()
                    {
                        Text = "Map",
                        Font = BasicFonts.GetFont(16),
                    }
                }
            };
            foreach(var mapName in MapBuilder.GetMaps())
            {
                var map = MapBuilder.GetMap(mapName);
                _mapSelectionStackPanel.Children.Add(new BorderControl()
                {
                    Margin = 5,
                    Child = new ButtonControl(clicked: SelectMap_Click)
                    {
                        Text = map.Name,
                        Font = BasicFonts.GetFont(10),
                        FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
                        FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                        Tag = mapName
                    }
                });
            }

            _gameStyleSelectionStackPanel = new StackPanelControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                Margin = 5,
                Row = 1,
                Column = 2,
                Children = new List<IControl>()
                {
                    new LabelControl()
                    {
                        Text = "Difficulty",
                        Font = BasicFonts.GetFont(16),
                    }
                }
            };
            foreach (var gameStyleName in GameStyleBuilder.GetGameStyles())
            {
                var style = GameStyleBuilder.GetGameStyle(gameStyleName);
                _gameStyleSelectionStackPanel.Children.Add(new BorderControl()
                {
                    Margin = 5,
                    Child = new ButtonControl(clicked: SelectGameStyle_Click)
                    {
                        Text = style.Name,
                        Font = BasicFonts.GetFont(10),
                        FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
                        FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                        Tag = gameStyleName
                    }
                });
            }

            _mapPreviewPanel = new CanvasControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.LightBlue)
            };

            Container = new GridControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                Margin = 5,
                RowDefinitions = new List<int>() { 1, 4, 1, 1 },
                ColumnDefinitions = new List<int>() { 1, 4, 4, 6, 1 },
                Children = new List<IControl>()
                {
                    new LabelControl()
                    {
                        Column = 1,
                        ColumnSpan = 3,
                        Text = "Game Setup",
                        FontColor = Color.Black,
                        Font = BasicFonts.GetFont(48)
                    },
                    _mapSelectionStackPanel,
                    _gameStyleSelectionStackPanel,
                    new BorderControl()
                    {
                        Row = 1,
                        Column = 3,
                        BorderWidth = 5,
                        Margin = 5,
                        Child = _mapPreviewPanel
                    },
                    new BorderControl()
                    {
                        Row = 2,
                        Column = 1,
                        Child = new ButtonControl(clicked: (x) => Parent.SwitchView(new MainMenu.MainMenu(Parent)))
                        {
                            Text = "Back",
                            Font = BasicFonts.GetFont(24),
                            FillColor = BasicTextures.GetBasicRectange(Color.White),
                            FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
                        },
                        Margin = 5
                    },
                    new BorderControl()
                    {
                        Row = 2,
                        Column = 3,
                        Child = new ButtonControl(clicked: StartButton_Click)
                        {
                            Text = "Start",
                            Font = BasicFonts.GetFont(24),
                            FillColor = BasicTextures.GetBasicRectange(Color.White),
                            FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
                        },
                        Margin = 5
                    }
                }
            };
            base.Initialize();
        }
    }
}
