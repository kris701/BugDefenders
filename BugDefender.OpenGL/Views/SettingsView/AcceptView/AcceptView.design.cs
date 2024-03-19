using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BugDefender.OpenGL.Screens.SettingsView.AcceptView
{
    public partial class AcceptView : BaseBugDefenderView
    {
        private LabelControl _timeLeftLabel;

        [MemberNotNull(nameof(_timeLeftLabel))]
        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = IWindow.BaseScreenSize.X,
                Height = IWindow.BaseScreenSize.Y
            });

            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Accept Changes?",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Settings will reset in 10 seconds otherwise.",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });
            _timeLeftLabel = new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 500,
                Text = "10 seconds left",
                Font = BasicFonts.GetFont(48),
                FontColor = Color.Red
            };
            AddControl(0, _timeLeftLabel);

            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Accept", (e) => { Accept(); }));
            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Cancel", (e) => { Cancel(); }));
#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new AcceptView(Parent, _oldSettings, _newSettings)))
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
