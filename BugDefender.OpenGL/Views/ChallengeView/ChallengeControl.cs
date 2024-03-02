using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.ChallengeView
{
    public class ChallengeControl : TileControl
    {
        public ChallengeDefinition Challenge { get; }

        private readonly LabelControl _titleControl;
        private readonly TextboxControl _descriptionTextbox;
        private readonly ButtonControl _startButton;
        public ChallengeControl(GameWindow parent, ChallengeDefinition challenge, ClickedHandler click) : base(parent)
        {
            Challenge = challenge;
            Width = 800;
            Height = 120;
            _titleControl = new LabelControl(Parent)
            {
                X = 100,
                Width = 400,
                Height = 50,
                Text = challenge.Name,
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
            };
            _descriptionTextbox = new TextboxControl(Parent)
            {
                X = 100,
                Width = 800,
                Height = 75,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = Challenge.Description,
                FillColor = Parent.UIResources.GetTexture(new Guid("61bcf9c3-a78d-4521-8534-5690bdc2d6db"))
            };
            _startButton = new ButtonControl(Parent, click)
            {
                X = 500,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = $"[Reward: {challenge.Reward} credits] Start!",
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759"))
            };
        }

        public override void Initialize()
        {
            base.Initialize();
            _titleControl._y = _y;
            _titleControl.Initialize();

            _descriptionTextbox._y = _y + Scale(50);
            _descriptionTextbox.Initialize();

            _startButton._y = _y;
            _startButton.Tag = Tag;
            _startButton.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _titleControl.Update(gameTime);
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
            _descriptionTextbox.Draw(gameTime, spriteBatch);
            _startButton.Draw(gameTime, spriteBatch);
        }
    }
}
