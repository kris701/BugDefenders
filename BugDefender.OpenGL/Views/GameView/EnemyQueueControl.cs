using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.ResourcePacks.EntityResources;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EnemyQueueControl : TileControl
    {
        private readonly AnimatedTileControl _iconControl;
        private readonly TextboxControl _descriptionControl;
        public EnemyQueueControl(UIEngine parent) : base(parent)
        {
            _iconControl = new AnimatedTileControl(Parent)
            {
                Width = 25,
                Height = 25,
            };
            _descriptionControl = new TextboxControl(Parent)
            {
                Margin = 1,
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Height = 50,
                Width = 150,
                Text = ""
            };
        }

        public void UpdateToEnemy(List<Guid> wave, float evolution)
        {
            var def = ResourceManager.Enemies.GetResource(wave[0]);
            var instance = new EnemyInstance(def, evolution);
            var animation = Parent.UIResources.GetAnimation<EnemyEntityDefinition>(def.ID);
            var textureSet = Parent.UIResources.GetTextureSet(animation.OnCreate);
            _iconControl.TileSet = textureSet.LoadedContents;
            _iconControl.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);

            var sb = new StringBuilder();
            if (wave.Count > 1)
            {
                sb.AppendLine($"{def.Name}+{wave.Count - 1} more");
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

            _descriptionControl.Text = sb.ToString();
        }

        public override void Initialize()
        {
            _iconControl._x = _x + Scale(10);
            _iconControl._y = _y + Height / 2 - _iconControl.Height / 2;
            _iconControl.Initialize();

            _descriptionControl._x = _x + Scale(10) + _iconControl.Width;
            _descriptionControl._y = _y + Scale(10);
            _descriptionControl.Initialize();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _iconControl.Update(gameTime);
            _descriptionControl.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            _iconControl.Draw(gameTime, spriteBatch);
            _descriptionControl.Draw(gameTime, spriteBatch);
        }
    }
}
