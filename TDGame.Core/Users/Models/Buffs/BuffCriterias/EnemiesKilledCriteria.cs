using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Users.Models.Buffs.BuffCriterias
{
    public class EnemiesKilledCriteria : IBuffCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats)
        {
            if (stats.TotalKills >= Quantity)
                return true;
            return false;
        }
    }
}
