using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures.Animations;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EnemyQueueControl : TileControl
    {
        private AnimatedTileControl iconControl;
        private TextboxControl descriptionControl;
        public EnemyQueueControl(IScreen parent) : base(parent)
        {
        }

        public void UpdateToEnemy(EnemyInstance enemy)
        {
            var animation = TextureManager.GetAnimation<EnemyAnimationDefinition>(enemy.DefinitionID);
            var textureSet = TextureManager.GetTextureSet(animation.OnCreate);
            iconControl.TileSet = textureSet.LoadedContents;
            iconControl.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
            var sb = new StringBuilder();
            sb.AppendLine(enemy.GetDefinition().Name);
            sb.AppendLine(enemy.GetDefinition().Description);
            sb.AppendLine($"Type: {ResourceManager.EnemyTypes.GetResource(enemy.GetDefinition().EnemyType).Name}");
            sb.AppendLine($"HP: {Math.Round(enemy.Health, 0)}");
            descriptionControl.Text = sb.ToString();
        }

        public override void Initialize()
        {
            iconControl = new AnimatedTileControl(Parent)
            {
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
            iconControl.Update(gameTime);
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
