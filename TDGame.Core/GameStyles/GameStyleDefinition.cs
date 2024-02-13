using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.GameStyles
{
    public class GameStyleDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float EvolutionRate { get; set; }
        public double EnemySpeedMultiplier { get; set; }
        public double MoneyMultiplier { get; set; }
        public int StartingHP { get; set; }
        public int StartingMoney { get; set; }
        public double EnemyWaveMultiplier { get; set; }
        public int ProjectileSpeedCap { get; set; }
        public int BossEveryNWave {  get; set; }
    }
}
