﻿namespace BugDefender.Core.Game.Models.GameStyles
{
    public class GameStyleDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float EvolutionRate { get; set; }
        public float EnemySpeedMultiplier { get; set; }
        public float RewardMultiplier { get; set; }
        public int StartingHP { get; set; }
        public int StartingMoney { get; set; }
        public float EnemyWaveMultiplier { get; set; }
        public int ProjectileSpeedCap { get; set; }
        public int BossEveryNWave { get; set; }
        public List<Guid> TurretBlackList { get; set; }
        public List<Guid> EnemyBlackList { get; set; }
        public List<Guid> TurretWhiteList { get; set; }
        public List<Guid> EnemyWhiteList { get; set; }
        public int MoneyPrWave { get; set; }
        public float TurretRefundPenalty { get; set; }
        public bool CampaignOnly { get; set; }

        public GameStyleDefinition(Guid iD, string name, string description, float evolutionRate, float enemySpeedMultiplier, float rewardMultiplier, int startingHP, int startingMoney, float enemyWaveMultiplier, int projectileSpeedCap, int bossEveryNWave, List<Guid> turretBlackList, List<Guid> enemyBlackList, List<Guid> turretWhiteList, List<Guid> enemyWhiteList, int moneyPrWave, float turretRefundPenalty, bool campaignOnly)
        {
            ID = iD;
            Name = name;
            Description = description;
            EvolutionRate = evolutionRate;
            EnemySpeedMultiplier = enemySpeedMultiplier;
            RewardMultiplier = rewardMultiplier;
            StartingHP = startingHP;
            StartingMoney = startingMoney;
            EnemyWaveMultiplier = enemyWaveMultiplier;
            ProjectileSpeedCap = projectileSpeedCap;
            BossEveryNWave = bossEveryNWave;
            TurretBlackList = turretBlackList;
            EnemyBlackList = enemyBlackList;
            TurretWhiteList = turretWhiteList;
            EnemyWhiteList = enemyWhiteList;
            MoneyPrWave = moneyPrWave;
            TurretRefundPenalty = turretRefundPenalty;
            CampaignOnly = campaignOnly;
        }

        public float GetDifficultyRating()
        {
            float difficulty = 1;
            difficulty += 1 / (float)StartingHP;
            difficulty += (1 / (float)StartingMoney) * 100;
            difficulty += 1 / (float)BossEveryNWave;
            difficulty += 1 / (1 + (float)MoneyPrWave);
            difficulty += 1 / TurretRefundPenalty;
            difficulty *= (1 + TurretBlackList.Count);
            difficulty *= EnemyWaveMultiplier;

            return difficulty;
        }
    }
}
