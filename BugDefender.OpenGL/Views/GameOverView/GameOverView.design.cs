using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BugDefender.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverView : BaseBugDefenderView
    {
        private TextboxControl _statsTextBox;

        [MemberNotNull(nameof(_statsTextBox))]
        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = IWindow.BaseScreenSize.X,
                Height = IWindow.BaseScreenSize.Y
            });

            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 150,
                FontColor = Color.White,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx48),
                Text = _title
            });

            AddControl(1, new BorderControl(new TileControl()
            {
                X = 100,
                Y = 250,
                Width = 960,
                Height = 540,
                FillColor = _screen
            })
            {
                Thickness = 5,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue)
            });

            AddControl(1, new TileControl()
            {
                X = 1100,
                Y = 250,
                Height = 540,
                Width = 700,
                FillColor = Parent.Textures.GetTexture(new Guid("02f8c9e2-e4c0-4310-934a-62c84cbb7384")),
            });
            AddControl(1, new LabelControl()
            {
                X = 1450,
                Y = 280,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                FontColor = Color.White,
                Text = "Results"
            });
            _statsTextBox = new TextboxControl()
            {
                X = 1100,
                Y = 300,
                Height = 540,
                Width = 700,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                FontColor = Color.White
            };
            AddControl(1, _statsTextBox);

            AddControl(1, new BugDefenderButtonControl(Parent, clicked: (s) => { SwitchView(new MainMenu.MainMenuView(Parent)); })
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 860,
                Width = 400,
                Height = 80,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                FontColor = Color.White,
                Text = "Main Menu",
                FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
            });

#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new GameOverView(Parent, _screen, _stats, _title)))
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
    }
}
