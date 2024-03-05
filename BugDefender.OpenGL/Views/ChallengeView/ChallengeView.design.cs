﻿using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views.ChallengeView;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;

namespace BugDefender.OpenGL.Screens.ChallengeView
{
    public partial class ChallengeView : BaseAnimatedView
    {
        private LabelControl _waitLabel;
        public override void Initialize()
        {
            BasicMenuPage.GenerateBaseMenu(
                this,
                Parent.UIResources.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
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

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 980,
                X = 1670,
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
