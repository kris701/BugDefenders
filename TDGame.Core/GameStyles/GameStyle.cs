using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.GameStyles
{
    public class GameStyle
    {
        public string Name { get; set; }
        public double EvolutionRate { get; set; }
        public double EnemySpeedMultiplier { get; set; }
        public double MoneyMultiplier { get; set; }

        public GameStyle(string name, double evolutionRate, double enemySpeedMultiplier, double moneyMultiplier)
        {
            Name = name;
            EvolutionRate = evolutionRate;
            EnemySpeedMultiplier = enemySpeedMultiplier;
            MoneyMultiplier = moneyMultiplier;
        }
    }
}
