using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.Core.EnemyTypes;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Resources;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EnemyQueueControl : TileControl
    {
        private TileControl iconControl;
        private TextboxControl descriptionControl;
        public EnemyQueueControl(IScreen parent) : base(parent)
        {
        }

        public void UpdateToEnemy(EnemyInstance enemy)
        {
            iconControl.FillColor = TextureBuilder.GetTexture(enemy.DefinitionID);
            var sb = new StringBuilder();
            sb.AppendLine(enemy.GetDefinition().Name);
            sb.AppendLine(enemy.GetDefinition().Description);
            sb.AppendLine($"Type: {ResourceManager.EnemyTypes.GetResource(enemy.GetDefinition().EnemyType).Name}");
            sb.AppendLine($"HP: {Math.Round(enemy.Health,0)}");
            descriptionControl.Text = sb.ToString();
        }

        public override void Initialize()
        {
            iconControl = new TileControl(Parent)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                Width = 25,
                Height = 25,
            };
            iconControl._x = _x + Parent.Scale(5);
            iconControl._y = _y + Parent.Scale(5) + Height / 2 - iconControl.Height / 2;
            iconControl.Initialize();

            descriptionControl = new TextboxControl(Parent)
            {
                Margin = 1,
                Font = BasicFonts.GetFont(10),
                FillColor = BasicTextures.GetBasicRectange(Color.DarkCyan),
                Height = 50,
                Width = 150,
                Text = ""
            };

            descriptionControl._x = _x + Parent.Scale(10) + iconControl.Width;
            descriptionControl._y = _y + Parent.Scale(10);
            descriptionControl.Initialize();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            descriptionControl.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            iconControl.Draw(gameTime, spriteBatch);
            descriptionControl.Draw(gameTime, spriteBatch);
        }
    }
}
