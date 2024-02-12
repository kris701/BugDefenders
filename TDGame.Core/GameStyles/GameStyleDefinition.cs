using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.GameStyles
{
    public class GameStyleDefinition
    {
        public string Name { get; set; }
        public double EvolutionRate { get; set; }
        public double EnemySpeedMultiplier { get; set; }
        public double MoneyMultiplier { get; set; }
        public int StartingHP { get; set; }
        public int StartingMoney { get; set; }
        public double EnemyWaveMultiplier { get; set; }
        public int ProjectileSpeedCap { get; set; }
        public int BossEveryNWave {  get; set; }
    }
}
