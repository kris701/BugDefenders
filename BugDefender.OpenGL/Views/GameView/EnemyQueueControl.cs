using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Resources;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EnemyQueueControl : CollectionControl
    {
        private List<Guid> _currentWave = new List<Guid>();
        private float _currentEvolution = -1;

        private readonly BugDefenderGameWindow _parent;
        private readonly AnimatedTileControl _iconControl;
        private readonly TextboxControl _descriptionControl;
        private readonly BugDefenderButtonControl _background;
        public EnemyQueueControl(BugDefenderGameWindow parent, ClickedHandler clicked)
        {
            _parent = parent;
            Width = 220;
            Height = 70;

            _background = new BugDefenderButtonControl(parent, clicked)
            {
                X = 0,
                Y = 0,
                Width = Width,
                Height = Height,
                FillColor = parent.Textures.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8"))
            };
            Children.Add(_background);
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
                Font = parent.Fonts.GetFont(FontSizes.Ptx8),
                FontColor = Color.White,
                Height = 50,
                Width = 150,
                Text = ""
            };
            Children.Add(_descriptionControl);
        }

        public void UpdateToEnemy(List<Guid> wave, float evolution)
        {
            if (!IsNew(wave, evolution))
                return;

            _currentWave = wave;
            _currentEvolution = evolution;

            var def = ResourceManager.Enemies.GetResource(wave[0]);
            var instance = new EnemyInstance(def, evolution);
            _background.Tag = instance;
            var animation = _parent.ResourcePackController.GetAnimation<EnemyEntityDefinition>(def.ID);
            var textureSet = _parent.Textures.GetTextureSet(animation.OnCreate);
            _iconControl.TileSet = textureSet.GetLoadedContent();
            _iconControl.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
            _iconControl.Frame = 0;
            _iconControl.FillColor = _iconControl.TileSet[0];

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

        private bool IsNew(List<Guid> wave, float evolution)
        {
            if (evolution != _currentEvolution)
                return true;
            if (wave.Count != _currentWave.Count)
                return true;
            for (int i = 0; i < _currentWave.Count; i++)
                if (wave[i] != _currentWave[i])
                    return true;

            return false;
        }
    }
}
