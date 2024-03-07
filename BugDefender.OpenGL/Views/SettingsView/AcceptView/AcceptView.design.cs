using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using System;

namespace BugDefender.OpenGL.Screens.SettingsView.AcceptView
{
    public partial class AcceptView : BaseBugDefenderView
    {
        private LabelControl _timeLeftLabel;
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

            AddControl(0, new ButtonControl(Parent, clicked: (x) => { Accept(); })
            {
                X = 50,
                Y = 980,
                Width = 200,
                Height = 50,
                Text = "Apply",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

            AddControl(0, new ButtonControl(Parent, clicked: (x) => { Cancel(); })
            {
                Y = 980,
                X = 1670,
                Width = 200,
                Height = 50,
                Text = "Cancel",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
