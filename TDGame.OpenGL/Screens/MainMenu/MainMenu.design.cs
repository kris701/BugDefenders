using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Screens.PathTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Engine.Helpers;

namespace Project1.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public override void Initialize()
        {
            Container = new GridControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                Margin = 5,
                RowDefinitions = new List<int>() { 2, 1, 1, 2 },
                ColumnDefinitions = new List<int>() { 1, 2, 1 },
                Children = new List<IControl>()
                {
                    new LabelControl()
                    {
                        Row = 1,
                        Column = 1,
                        Text = "Test",
                        FontColor = Color.Black,
                        Font = BasicFonts.GetFont(72)
                    },
                    new StackPanelControl()
                    {
                        FillColor = BasicTextures.GetBasicRectange(Color.White),
                        Margin = 5,
                        Row = 2,
                        Column = 1,
                        Children = new List<IControl>()
                        {
                            new BorderControl()
                            {
                                Child = new ButtonControl(clicked: (x) => Parent.SwitchView(new PathTestScreen(Parent)))
                                {
                                    Text = "Start Game",
                                    Font = BasicFonts.GetFont(24),
                                    FillColor = BasicTextures.GetBasicRectange(Color.White),
                                    FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
                                },
                                Margin = 5
                            },
                            new BorderControl()
                            {
                                Child = new ButtonControl(clicked: (x) => Parent.Exit())
                                {
                                    Text = "Exit",
                                    Font = BasicFonts.GetFont(24),
                                    FillColor = BasicTextures.GetBasicRectange(Color.White),
                                    FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
                                },
                                Margin = 5
                            }
                        }
                    }
                }
            };
            base.Initialize();
        }
    }
}
