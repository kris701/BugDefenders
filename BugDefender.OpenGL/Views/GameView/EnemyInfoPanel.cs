using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Views.GameView
{
    public class EnemyInfoPanel : CollectionControl
    {
        private TextboxControl _enemyStatsTextbox;

        public EnemyInfoPanel(BugDefenderGameWindow parent)
        {
            Width = 650;
            Height = 330;
            Children.Add(new TileControl()
            {
                FillColor = parent.TextureController.GetTexture(new Guid("90447608-bd7a-478c-9bfd-fddb26c731b7")),
                Height = Height,
                Width = Width
            });
            Children.Add(new LabelControl()
            {
                Text = "Enemy Stats",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Y = 5,
                Height = 35,
                Width = Width
            });

            _enemyStatsTextbox = new TextboxControl()
            {
                Font = BasicFonts.GetFont(8),
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
