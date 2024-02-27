namespace TDGame.Core.Game.Models.GameStyles
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
        public int MoneyPrWave { get; set; }

        public GameStyleDefinition(Guid iD, string name, string description, float evolutionRate, float enemySpeedMultiplier, float moneyMultiplier, int startingHP, int startingMoney, float enemyWaveMultiplier, int projectileSpeedCap, int bossEveryNWave, List<Guid> turretBlackList, List<Guid> enemyBlackList, int moneyPrWave)
        {
            ID = iD;
            Name = name;
            Description = description;
            EvolutionRate = evolutionRate;
            EnemySpeedMultiplier = enemySpeedMultiplier;
            MoneyMultiplier = moneyMultiplier;
            StartingHP = startingHP;
            StartingMoney = startingMoney;
            EnemyWaveMultiplier = enemyWaveMultiplier;
            ProjectileSpeedCap = projectileSpeedCap;
            BossEveryNWave = bossEveryNWave;
            TurretBlackList = turretBlackList;
            EnemyBlackList = enemyBlackList;
            MoneyPrWave = moneyPrWave;
        }
    }
}
