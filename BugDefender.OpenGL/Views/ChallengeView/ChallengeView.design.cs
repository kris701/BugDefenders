using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views.ChallengeView;
using Microsoft.Xna.Framework;
using System;

namespace BugDefender.OpenGL.Screens.ChallengeView
{
    public partial class ChallengeView : BaseAnimatedView
    {
        private LabelControl _waitLabel;
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Todays Challenges",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            _waitLabel = new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"{_remainingChallenges.Count} challenges remaining for today. Until reroll",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            };
            AddControl(0, _waitLabel);

            int count = 0;
            foreach (var id in _remainingChallenges)
            {
                var newControl = new ChallengeControl(
                    Parent,
                    ResourceManager.Challenges.GetResource(id),
                    StartButton_Click,
                    Parent.CurrentUser.CompletedChallenges.Contains(id))
                {
                    X = 50,
                    Y = 210 + count++ * 135 + 5,
                    Tag = id
                };
                AddControl(1, newControl);
            }

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new ChallengeView(Parent)))
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
