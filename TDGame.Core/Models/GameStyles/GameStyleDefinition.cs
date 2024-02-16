﻿namespace TDGame.Core.Models.GameStyles
{
    public class GameStyleDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float EvolutionRate { get; set; }
        public float EnemySpeedMultiplier { get; set; }
        public float MoneyMultiplier { get; set; }
        public int StartingHP { get; set; }
        public int StartingMoney { get; set; }
        public float EnemyWaveMultiplier { get; set; }
        public int ProjectileSpeedCap { get; set; }
        public int BossEveryNWave { get; set; }
        public List<Guid> TurretBlackList { get; set; }
        public List<Guid> EnemyBlackList { get; set; }
    }
}
