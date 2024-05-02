using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.OpenGL.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using System;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EnemyInfoPanel : CollectionControl
    {
        private readonly TextboxControl _enemyStatsTextbox;

        public EnemyInfoPanel(BugDefenderGameWindow parent)
        {
            Width = 650;
            Height = 330;
            Children.Add(new TileControl()
            {
                FillColor = parent.Textures.GetTexture(new Guid("90447608-bd7a-478c-9bfd-fddb26c731b7")),
                Height = Height,
                Width = Width
            });
            Children.Add(new LabelControl()
            {
                Text = "Enemy Stats",
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                Y = 5,
                Height = 35,
                Width = Width
            });

            _enemyStatsTextbox = new TextboxControl()
            {
                Font = parent.Fonts.GetFont(FontSizes.Ptx8),
                FontColor = Color.White,
                Text = "Select an Enemy",
                X = 10,
                Y = 50,
                Width = Width - 20,
                Height = Height - 115
            };
            Children.Add(_enemyStatsTextbox);
        }

        public void Unselect()
        {
            _enemyStatsTextbox.Text = "Select an Enemy";
        }

        public void Select(EnemyInstance enemy)
        {
            _enemyStatsTextbox.Text = enemy.ToString();
        }
    }
}
