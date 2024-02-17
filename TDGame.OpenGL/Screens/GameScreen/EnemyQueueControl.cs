﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void UpdateToEnemy(List<Guid> wave, float evolution)
        {
            var def = ResourceManager.Enemies.GetResource(wave[0]);
            var instance = new EnemyInstance(def, evolution);
            var animation = TextureManager.GetAnimation<EnemyAnimationDefinition>(def.ID);
            var textureSet = TextureManager.GetTextureSet(animation.OnCreate);
            iconControl.TileSet = textureSet.LoadedContents;
            iconControl.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);

            var sb = new StringBuilder();
            if (wave.Count > 1)
            {
                sb.AppendLine($"{def.Name} + {wave.Count - 1} more");
                sb.Append($"Types: {ResourceManager.EnemyTypes.GetResource(def.EnemyType).Name}");
                foreach(var enemy in wave.Skip(1))
                    sb.Append($", {ResourceManager.EnemyTypes.GetResource(ResourceManager.Enemies.GetResource(enemy).EnemyType).Name}");
                sb.AppendLine();
                var hp = instance.Health;
                foreach (var enemy in wave.Skip(1))
                    hp += new EnemyInstance(ResourceManager.Enemies.GetResource(enemy), evolution).Health;
                sb.AppendLine($"Total HP: {Math.Round(hp, 0)}");
            }
            else
            {
                sb.AppendLine(def.Name);
                sb.AppendLine(def.Description);
                sb.AppendLine($"Type: {ResourceManager.EnemyTypes.GetResource(def.EnemyType).Name}");
                sb.AppendLine($"HP: {Math.Round(instance.Health, 0)}");
            }

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
                Font = BasicFonts.GetFont(8),
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
