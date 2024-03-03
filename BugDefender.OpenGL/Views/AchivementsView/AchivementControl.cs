using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace BugDefender.OpenGL.Views.AchivementsView
{
    public class AchivementControl : TileControl
    {
        public AchivementDefinition Achivement { get; }

        private readonly TileControl _iconTile;
        private readonly LabelControl _titleControl;
        private readonly TextboxControl _requirementsTextBox;
        private readonly TextboxControl _descriptionTextbox;
        public AchivementControl(GameWindow parent, AchivementDefinition achivement, bool isUnlocked)
        {
            Achivement = achivement;
            Width = 900;
            Height = 140;
            _iconTile = new TileControl()
            {
                X = 75,
                Width = 75,
                Height = 75,
                FillColor = parent.UIResources.GetTexture(Achivement.ID)
            };
            _titleControl = new LabelControl()
            {
                X = 160,
                Width = 400,
                Height = 35,
                Text = achivement.Name,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
            };
            var sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            foreach (var req in achivement.Criterias)
                sb.AppendLine(req.GetDescriptionString());
            _requirementsTextBox = new TextboxControl()
            {
                X = 560,
                Width = 400,
                Height = 75,
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Text = sb.ToString(),
                Margin = 15
            };
            _descriptionTextbox = new TextboxControl()
            {
                X = 160,
                Width = 400,
                Height = 75,
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Text = achivement.Description,
                Margin = 15
            };

            if (isUnlocked)
            {
                if (parent.CurrentUser.Achivements.Contains(achivement.ID))
                    FillColor = parent.UIResources.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));
                else
                    FillColor = parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
            }
            else
                FillColor = parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8"));
        }

        public override void Initialize()
        {
            base.Initialize();
            _iconTile.Y = Y + 30;
            _iconTile.Initialize();

            _titleControl.Y = Y + 20;
            _titleControl.Initialize();

            _descriptionTextbox.Y = Y + 45;
            _descriptionTextbox.Initialize();

            _requirementsTextBox.Y = Y + 10;
            _requirementsTextBox.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _titleControl.Update(gameTime);
            _descriptionTextbox.Update(gameTime);
            _requirementsTextBox.Update(gameTime);
            _iconTile.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            _titleControl.Draw(gameTime, spriteBatch);
            _iconTile.Draw(gameTime, spriteBatch);
            _descriptionTextbox.Draw(gameTime, spriteBatch);
            _requirementsTextBox.Draw(gameTime, spriteBatch);
        }
    }
}
