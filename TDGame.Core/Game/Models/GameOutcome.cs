using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Game.Models
{
    public class GameOutcome
    {
        public int TotalKills { get; set; } = 0;
        public Dictionary<Guid, int> KillsOfType { get; set; } = new Dictionary<Guid, int>();
        public int TotalTurretsPlaced { get; set; } = 0;
        public Dictionary<Guid, int> TotalTurretsPlacedOfType { get; set; } = new Dictionary<Guid, int>();
    }
}
