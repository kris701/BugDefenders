﻿using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.ChallengeView
{
    public class ChallengeControl : TileControl
    {
        public ChallengeDefinition Challenge { get; }

        private readonly LabelControl _titleControl;
        private readonly TextboxControl _requirementsTextBox;
        private readonly TextboxControl _descriptionTextbox;
        private readonly ButtonControl _startButton;
        public ChallengeControl(GameWindow parent, ChallengeDefinition challenge, ClickedHandler click, bool isFinished) : base(parent)
        {
            Challenge = challenge;
            Width = 900;
            Height = 140;
            _titleControl = new LabelControl(Parent)
            {
                X = 75,
                Width = 400,
                Height = 35,
                Text = challenge.Name,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
            };
            var sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            foreach (var req in challenge.Criterias)
                sb.AppendLine(req.GetDescriptionString());
            _requirementsTextBox = new TextboxControl(Parent)
            {
                X = 525,
                Width = 400,
                Height = 75,
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Text = sb.ToString(),
                Margin = 15
            };
            var sb2 = new StringBuilder();
            sb2.AppendLine(Challenge.Description);
            sb2.AppendLine($"Map: '{ResourceManager.Maps.GetResource(challenge.MapID).Name}'. Gamestyle: '{ResourceManager.GameStyles.GetResource(challenge.GameStyleID).Name}'");
            _descriptionTextbox = new TextboxControl(Parent)
            {
                X = 75,
                Width = 400,
                Height = 75,
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Text = sb2.ToString(),
                Margin = 15
            };
            _startButton = new ButtonControl(Parent, click)
            {
                X = 525,
                Width = 400,
                Height = 35,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                Text = $"[Reward: {challenge.Reward} credits] Start!",
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                IsEnabled = !isFinished
            };
            if (isFinished)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));
                _startButton.Text = "Completed!";
            }
            else
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
        }

        public override void Initialize()
        {
            base.Initialize();
            _titleControl._y = _y + Scale(20);
            _titleControl.Initialize();

            _requirementsTextBox._y = _y + Scale(45);
            _requirementsTextBox.Initialize();

            _descriptionTextbox._y = _y + Scale(45);
            _descriptionTextbox.Initialize();

            _startButton._y = _y + Scale(20);
            _startButton.Tag = Tag;
            _startButton.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _titleControl.Update(gameTime);
            _requirementsTextBox.Update(gameTime);
            _descriptionTextbox.Update(gameTime);
            _startButton.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            _titleControl.Draw(gameTime, spriteBatch);
            _requirementsTextBox.Draw(gameTime, spriteBatch);
            _descriptionTextbox.Draw(gameTime, spriteBatch);
            _startButton.Draw(gameTime, spriteBatch);
        }
    }
}
