using BugDefender.Core.Resources;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.ChallengeView;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BugDefender.OpenGL.Screens.ChallengeView
{
    public partial class ChallengeView : BaseBugDefenderView
    {
        private LabelControl _waitLabel;
        [MemberNotNull(nameof(_waitLabel))]
        public override void Initialize()
        {
            BasicMenuHelper.GenerateBaseMenu(
                this,
                Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Todays Challenges",
                $"{_remainingChallenges.Count} challenges for today. Time until reroll:");

            _waitLabel = new LabelControl()
            {
                Y = 175,
                X = 1100,
                Height = 35,
                Width = 400,
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            };
            AddControl(0, _waitLabel);
            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 200,
                Height = 50,
                Font = BasicFonts.GetFont(16),
                Text = "Overwrites existing challenge save!",
                FontColor = Color.Red,
                IsVisible = Parent.UserManager.SaveExists("Latest Challenge")
            });

            int count = 0;
            foreach (var id in _remainingChallenges)
            {
                var newControl = new ChallengeControl(
                    Parent,
                    ResourceManager.Challenges.GetResource(id),
                    StartButton_Click,
                    Parent.UserManager.CurrentUser.CompletedChallenges.Contains(id))
                {
                    X = 500,
                    Y = 250 + count++ * 135 + 5
                };
                AddControl(1, newControl);
            }

            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Back", (e) => { SwitchView(new MainMenu.MainMenuView(Parent)); }));

#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new ChallengeView(Parent)))
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
