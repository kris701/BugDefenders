using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EnemyQueueControl : CollectionControl
    {
        private readonly GameWindow _parent;
        private readonly AnimatedTileControl _iconControl;
        private readonly TextboxControl _descriptionControl;
        public EnemyQueueControl(GameWindow parent)
        {
            _parent = parent;
            Width = 220;
            Height = 70;

            var background = new TileControl()
            {
                X = 0,
                Y = 0,
                Width = Width,
                Height = Height,
                FillColor = parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9"))
            };
            Children.Add(background);
            _iconControl = new AnimatedTileControl()
            {
                X = 10,
                Y = Height / 2 - 25 / 2,
                Width = 25,
                Height = 25,
            };
            Children.Add(_iconControl);
            _descriptionControl = new TextboxControl()
            {
                X = 10 + _iconControl.Width,
                Y = 10,
                Margin = 3,
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Height = 50,
                Width = 150,
                Text = ""
            };
            Children.Add(_descriptionControl);
        }

        public void UpdateToEnemy(List<Guid> wave, float evolution)
        {
            var def = ResourceManager.Enemies.GetResource(wave[0]);
            var instance = new EnemyInstance(def, evolution);
            var animation = _parent.TextureController.GetAnimation<EnemyEntityDefinition>(def.ID);
            var textureSet = _parent.TextureController.GetTextureSet(animation.OnCreate);
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
    }
}
