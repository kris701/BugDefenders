using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using System;

namespace BugDefender.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverView : BaseAnimatedView
    {
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("1d50ad58-0503-4fc6-95e4-1d18abb8c485")),
                Width = 1000,
                Height = 1000
            });

            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                Text = "Game Over!",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(72),
            });
            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 225,
                Text = $"Score: {_score}",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(24),
            });
            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 275,
                Text = $"Played for {_gameTime.ToString("hh\\:mm\\:ss")}",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(24),
            });

            AddControl(1, new BorderControl(Parent, new TileControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 350,
                Width = 500,
                Height = 500,
                FillColor = _screen
            })
            {
                Thickness = 5,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue)
            });

            AddControl(1, new ButtonControl(Parent, clicked: (s) => { SwitchView(new MainMenu.MainMenuView(Parent)); })
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 875,
                Width = 400,
                Height = 80,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                Text = "Main Menu",
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new GameOverView(Parent, _screen, _score, _gameTime)))
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
    }
}
