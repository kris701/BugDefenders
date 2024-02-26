namespace TDGame.Core.Users.Models
{
    public class StatsDefinition
    {
        public int TotalKills { get; set; } = 0;
        public Dictionary<Guid, int> KillsOfType { get; set; } = new Dictionary<Guid, int>();
        public int TotalTurretsPlaced { get; set; } = 0;
        public Dictionary<Guid, int> TotalTurretsPlacedOfType { get; set; } = new Dictionary<Guid, int>();
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
            foreach(var key in outcome.TotalTurretKillsOfType.Keys)
            {
                if (!TotalTurretKillsOfType.ContainsKey(key))
                    TotalTurretKillsOfType.Add(key, new Dictionary<Guid, int>());
                if (!TotalTurretKills.ContainsKey(key))
                    TotalTurretKills.Add(key,0);
                TotalTurretKills[key] += outcome.TotalTurretKills[key];

                foreach (var turret in outcome.TotalTurretKillsOfType[key].Keys)
                {
                    if (!TotalTurretKillsOfType[key].ContainsKey(turret))
                        TotalTurretKillsOfType[key].Add(turret, 0);
                    TotalTurretKillsOfType[key][turret] += outcome.TotalTurretKillsOfType[key][turret];
                }
            }
        }
    }
}
