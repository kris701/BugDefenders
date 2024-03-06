using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models
{
    [JsonSerializable(typeof(StatsDefinition))]
    public class StatsDefinition
    {
        public int TotalWavesStarted { get; set; } = 0;

        public int TotalMoneyEarned { get; set; } = 0;

        public int TotalKills { get; set; } = 0;
        public Dictionary<Guid, int> KillsOfType { get; set; } = new Dictionary<Guid, int>();

        public int TotalTurretsPlaced { get; set; } = 0;
        public Dictionary<Guid, int> TotalTurretsPlacedOfType { get; set; } = new Dictionary<Guid, int>();

        public int TotalTurretsSold { get; set; } = 0;
        public Dictionary<Guid, int> TotalTurretsSoldOfType { get; set; } = new Dictionary<Guid, int>();

        public int TotalTurretsUpgraded { get; set; } = 0;
        public Dictionary<Guid, int> TotalTurretsUpgradedOfType { get; set; } = new Dictionary<Guid, int>();

        public Dictionary<Guid, int> TotalTurretKills { get; set; } = new Dictionary<Guid, int>();
        public Dictionary<Guid, Dictionary<Guid, int>> TotalTurretKillsOfType { get; set; } = new Dictionary<Guid, Dictionary<Guid, int>>();

        public void Combine(StatsDefinition outcome)
        {
            TotalKills += outcome.TotalKills;
            foreach (var key in outcome.KillsOfType.Keys)
            {
                if (!KillsOfType.ContainsKey(key))
                    KillsOfType.Add(key, outcome.KillsOfType[key]);
                else
                    KillsOfType[key] += outcome.KillsOfType[key];
            }
            TotalTurretsPlaced += outcome.TotalTurretsPlaced;
            foreach (var key in outcome.TotalTurretsPlacedOfType.Keys)
            {
                if (!TotalTurretsPlacedOfType.ContainsKey(key))
                    TotalTurretsPlacedOfType.Add(key, outcome.TotalTurretsPlacedOfType[key]);
                else
                    TotalTurretsPlacedOfType[key] += outcome.TotalTurretsPlacedOfType[key];
            }
            foreach (var key in outcome.TotalTurretKillsOfType.Keys)
            {
                if (!TotalTurretKillsOfType.ContainsKey(key))
                    TotalTurretKillsOfType.Add(key, new Dictionary<Guid, int>());
                if (!TotalTurretKills.ContainsKey(key))
                    TotalTurretKills.Add(key, 0);
                TotalTurretKills[key] += outcome.TotalTurretKills[key];

                foreach (var turret in outcome.TotalTurretKillsOfType[key].Keys)
                {
                    if (!TotalTurretKillsOfType[key].ContainsKey(turret))
                        TotalTurretKillsOfType[key].Add(turret, 0);
                    TotalTurretKillsOfType[key][turret] += outcome.TotalTurretKillsOfType[key][turret];
                }
            }
        }

        public void MoneyEarned(int amount) => TotalMoneyEarned += amount;

        public void EnemyKilled(Guid enemyID, Guid fromTurretID)
        {
            TotalKills++;
            if (!KillsOfType.ContainsKey(enemyID))
                KillsOfType.Add(enemyID, 1);
            else
                KillsOfType[enemyID]++;

            if (!TotalTurretKills.ContainsKey(fromTurretID))
                TotalTurretKills.Add(fromTurretID, 1);
            else
                TotalTurretKills[fromTurretID]++;

            if (!TotalTurretKillsOfType.ContainsKey(fromTurretID))
                TotalTurretKillsOfType.Add(fromTurretID, new Dictionary<Guid, int>());
            if (!TotalTurretKillsOfType[fromTurretID].ContainsKey(enemyID))
                TotalTurretKillsOfType[fromTurretID].Add(enemyID, 1);
            else
                TotalTurretKillsOfType[fromTurretID][enemyID]++;
        }

        public void PlacedTurret(Guid turretID)
        {
            TotalTurretsPlaced++;
            if (!TotalTurretsPlacedOfType.ContainsKey(turretID))
                TotalTurretsPlacedOfType.Add(turretID, 1);
            else
                TotalTurretsPlacedOfType[turretID]++;
        }

        public void TurretSold(Guid turretID)
        {
            TotalTurretsSold++;
            if (!TotalTurretsSoldOfType.ContainsKey(turretID))
                TotalTurretsSoldOfType.Add(turretID, 1);
            else
                TotalTurretsSoldOfType[turretID]++;
        }

        public void TurretUpgraded(Guid turretID)
        {
            TotalTurretsUpgraded++;
            if (!TotalTurretsUpgradedOfType.ContainsKey(turretID))
                TotalTurretsUpgradedOfType.Add(turretID, 1);
            else
                TotalTurretsUpgradedOfType[turretID]++;
        }

        public void WaveStarted()
        {
            TotalWavesStarted++;
        }
    }
}
