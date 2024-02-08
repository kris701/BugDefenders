using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.FormEngine.Core.Screens;
using MonoGame.FormEngine.Toolbox.Controls;
using MonoGame.FormEngine.Toolbox.Helpers;
using MonoGame.FormEngine.Toolbox.Interfaces;
using Project1.Helpers;
using Project1.Screens.PathTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public override void Initialize()
        {
            Container = new GridControl()
            {
                FillColor = BasicTextures.Gray,
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
                        Font = BasicFonts.Font72pt
                    },
                    new StackPanelControl()
                    {
                        FillColor = BasicTextures.White,
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
                                    Font = BasicFonts.Font24pt,
                                    FillColor = BasicTextures.White,
                                    FillClickedColor = BasicTextures.Gray
                                },
                                Margin = 5
                            },
                            new BorderControl()
                            {
                                Child = new ButtonControl(clicked: (x) => Parent.Exit())
                                {
                                    Text = "Exit",
                                    Font = BasicFonts.Font24pt,
                                    FillColor = BasicTextures.White,
                                    FillClickedColor = BasicTextures.Gray
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
