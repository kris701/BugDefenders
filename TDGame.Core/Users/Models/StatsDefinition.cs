using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models;

namespace TDGame.Core.Users.Models
{
    public class StatsDefinition
    {
        public int TotalKills { get; set; } = 0;
        public Dictionary<Guid, int> KillsOfType { get; set; } = new Dictionary<Guid, int>();
        public int TotalTurretsPlaced { get; set; } = 0;
        public Dictionary<Guid, int> TotalTurretsPlacedOfType { get; set; } = new Dictionary<Guid, int>();

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
        }
    }
}
