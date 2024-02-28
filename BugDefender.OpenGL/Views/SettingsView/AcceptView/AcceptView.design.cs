using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using System;

namespace BugDefender.OpenGL.Screens.SettingsView.AcceptView
{
    public partial class AcceptView : BaseView
    {
        private LabelControl _timeLeftLabel;
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("0739c674-5f0e-497a-a619-8ba39fd545b3")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Accept Changes?",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Settings will reset in 10 seconds otherwise.",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });
            _timeLeftLabel = new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 500,
                Text = "10 seconds left",
                Font = BasicFonts.GetFont(48),
                FontColor = Color.Red
            };
            AddControl(0, _timeLeftLabel);

            AddControl(0, new ButtonControl(Parent, clicked: (x) => { Accept(); })
            {
                X = 50,
                Y = 900,
                Width = 200,
                Height = 50,
                Text = "Apply",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

            AddControl(0, new ButtonControl(Parent, clicked: (x) => { Cancel(); })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Cancel",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new AcceptView(Parent, _oldSettings, _newSettings)))
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
