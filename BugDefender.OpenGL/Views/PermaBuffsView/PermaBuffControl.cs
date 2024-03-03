using BugDefender.Core.Users.Models.Buffs;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.PermaBuffsView
{
    public class PermaBuffControl : TileControl
    {
        public BuffDefinition Buff { get; }

        private readonly LabelControl _titleControl;
        private readonly TextboxControl _requirementsTextBox;
        private readonly TextboxControl _descriptionTextbox;
        private readonly ButtonControl _buyButton;
        public PermaBuffControl(GameWindow parent, BuffDefinition buff, ClickedHandler click, bool isUnlocked) : base(parent)
        {
            Buff = buff;
            Width = 900;
            Height = 140;
            _titleControl = new LabelControl(Parent)
            {
                X = 75,
                Width = 400,
                Height = 35,
                Text = buff.Name,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
            };
            var sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            foreach (var req in buff.Criterias)
                sb.AppendLine(req.GetDescriptionString());
            _requirementsTextBox = new TextboxControl(Parent)
            {
                X = 525,
                Width = 400,
                Height = 75,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = sb.ToString(),
                Margin = 15
            };
            _descriptionTextbox = new TextboxControl(Parent)
            {
                X = 75,
                Width = 400,
                Height = 75,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = Buff.Description,
                Margin = 15
            };
            _buyButton = new ButtonControl(Parent, click)
            {
                X = 525,
                Width = 400,
                Height = 35,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                Text = $"[{buff.Cost} credits] Buy",
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Tag = buff,
                IsEnabled = isUnlocked && Parent.CurrentUser.Credits >= buff.Cost
            };
            if (isUnlocked)
            {
                if (Parent.CurrentUser.Buffs.Contains(buff.ID))
                {
                    FillColor = Parent.UIResources.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));
                    _buyButton.Text = "Owned!";
                }
                else
                    FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
            }
            else
                FillColor = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8"));
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

            _buyButton._y = _y + Scale(20);
            _buyButton.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _titleControl.Update(gameTime);
            _requirementsTextBox.Update(gameTime);
            _descriptionTextbox.Update(gameTime);
            _buyButton.Update(gameTime);
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
            _buyButton.Draw(gameTime, spriteBatch);
        }
    }
}
